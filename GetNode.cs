using HtmlAgilityPack;
using PS_parser.Sheets;
using Serilog;
using System.Net;
using Newtonsoft.Json;
using PS_parser.Config;

namespace PS_parser
{
    internal class GetNode
    {
        private static readonly IList<IList<object>> _gamesInfo = new List<IList<object>>();
        private static readonly object lockObject = new();
        private static readonly int threadCount = 15;
        private static CountdownEvent countdownEvent = new(threadCount);
        private static HttpClient proxyClient;

        public GetNode()
        {
            SetupHttpClient();
            SetupLogger();
        }

        private static void SetupHttpClient()
        {
            var proxyConfig = JsonConvert.DeserializeObject<Proxy>(File.ReadAllText($@"/root/parse/Config/proxy.json"));
            var proxyUri = new Uri(proxyConfig.ProxyUri);
            var proxyCredentials = new NetworkCredential(proxyConfig.ProxyLogin, proxyConfig.ProxyPassword);

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = new WebProxy(proxyUri)
                {
                    Credentials = proxyCredentials,
                },
                UseCookies = true,
                UseDefaultCredentials = true,
                UseProxy = true,
            };

            proxyClient = new HttpClient(httpClientHandler);
        }

        private static void SetupLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"parser/log.txt")
                .CreateLogger();
        }

        public async Task StartThreadsPS5()
        {
            var req = new Request();
            var google = new SheetsApi();
            await req.SendTelegram("Parsing Start");

            var getGames = new ParseGameId();
            var gamesId = await getGames.GetGamesRequest();
            var uniqueGames = gamesId.Distinct().ToList();

            var splitGames = SplitList(uniqueGames, threadCount);

            var tasks = new List<Task>();

            for (int i = 0; i < threadCount; i++)
            {
                var gameList = splitGames[i];
                tasks.Add(Task.Run(() => ReadGameInfo(gameList, i + 1)));
            }

            Console.WriteLine("All tasks started");
            await Task.WhenAll(tasks);

            Console.WriteLine("Write game data in google sheets...");
            google.WriteEntries(_gamesInfo);
            await req.SendTelegram("Table entry completed");
            Console.WriteLine("Table entry completed");
        }

        private static List<List<string>> SplitList(List<string> games, int parts)
        {
            int count = games.Count / parts;
            var splitGames = new List<List<string>>();

            for (int i = 0; i < parts; i++)
            {
                splitGames.Add(games.Skip(i * count).Take(count).ToList());
            }

            return splitGames;
        }

        private static async Task ReadGameInfo(List<string> gameList, int thread)
        {
            var helper = new Helpers();
            var htmlDoc = new HtmlDocument();
            var node = new GetNode();

            int res = 0;
            string gameInfo = string.Empty;

            try
            {
                foreach (var game in gameList)
                {
                    string htmlCode = await proxyClient.GetStringAsync($"https://store.playstation.com/en-tr/product/{game}");

                    htmlDoc.LoadHtml(htmlCode);
                    var gameTitle = htmlDoc.DocumentNode.SelectSingleNode("//h1[contains(@class,'psw-m-b-5 psw-t-title-l')]");
                    var priceFirstLine = htmlDoc.DocumentNode.SelectSingleNode("(//div[contains(@class,'psw-l-anchor psw-l-stack-left')]//span)[1]");
                    var priceSecondLine = htmlDoc.DocumentNode.SelectSingleNode("(//span[contains(@class,'psw-fill-x psw-l-line-left')])[2]");
                    var editionLine = htmlDoc.DocumentNode.SelectSingleNode("//span[@data-qa='mfe-game-title#productTag1']");
                    var voiceLine = htmlDoc.DocumentNode.SelectSingleNode("//dd[@data-qa='gameInfo#releaseInformation#voice-value']");
                    var subtitlesLine = htmlDoc.DocumentNode.SelectSingleNode("//dd[@data-qa='gameInfo#releaseInformation#subtitles-value']");
                    var releaseLine = htmlDoc.DocumentNode.SelectSingleNode("(//dd[contains(@class,'psw-p-r-6 psw-p-r-0@tablet-s')])[2]");
                    var discountOneLine = htmlDoc.DocumentNode.SelectSingleNode("//span[@data-qa='mfeCtaMain#offer0#discountInfo']");
                    var discountOneEndLine = htmlDoc.DocumentNode.SelectSingleNode("//span[@data-qa='mfeCtaMain#offer0#discountDescriptor']");
                    var discountTwoLine = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@class,'psw-t-overline psw-t-bold')]");
                    var versionLine = htmlDoc.DocumentNode.SelectSingleNode("//dd[@data-qa='gameInfo#releaseInformation#platform-value']");

                    string? headerText = gameTitle?.InnerText;
                    string? priceOne = priceFirstLine?.InnerText;
                    string? priceTwo = priceSecondLine?.InnerText;
                    string? edition = editionLine?.InnerText;
                    string? voice = voiceLine?.InnerText;
                    string? subtitles = subtitlesLine?.InnerText;
                    string? release = releaseLine?.InnerText;
                    string? discountOne = discountOneLine?.InnerText;
                    string? discountOneEnd = discountOneEndLine?.InnerText;
                    string? discountTwo = discountTwoLine?.InnerText;
                    string? version = versionLine?.InnerText;

                    var priceres = helper.SplitFirstPrice(priceOne);
                    gameInfo = $"{game} | {headerText}";
                    Console.WriteLine($"Thread {thread} position {res++}/{gameList.Count} | {game}:{headerText}");
                    Log.Information($"{game}:{headerText}");

                    node.sendGameData(game, headerText?.Replace("%amp;", ""), version, priceres, helper.slitSecondPrice(priceTwo),
                        helper.editionEdit(edition), voice, subtitles, release,
                        discountOne, discountOneEnd, helper.discountTwoTrim(discountTwo));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Exception in thread {thread}: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                countdownEvent.Signal();
            }
        }

        private void sendGameData(string game, string title, string version, string[] priceOne, string[] priceTwo, string edition, string voice,
            string subtitles, string release, string discountOne, string discountOneEnd, string discountTwo)
        {
            lock (lockObject)
            {
                _gamesInfo.Add(new List<object> {
                    game ?? "", $"https://store.playstation.com/en-tr/product/{game}/", title ?? "", version ?? "", edition ?? "",
                    priceOne[0], priceOne[1], discountOne ?? "", discountOneEnd ?? "", priceTwo[0], priceTwo[1], discountTwo ?? "",
                    "", release ?? "", "url", voice ?? "", subtitles ?? ""
                });
            }
        }
    }
}
