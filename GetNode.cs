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
        static IList<IList<object>> _gamesInfo = new List<IList<object>>();
        static object lockObject = new();
        static int threadCount = 15;
        static CountdownEvent countdownEvent = new(threadCount);

        public async Task StartThreadsPS5()
        {
            Request req = new();
            SheetsApi google = new();
            await req.SendTelegram("Parsing Start");
            ParseGameId getGames = new();
            List<string> gamesId = await getGames.GetGamesRequest();
            List<string> uniqueGames = gamesId.Distinct().ToList();

            int count = uniqueGames.Count / 15;

            List<string> _listThreadOne = uniqueGames.Take(count).ToList();
            List<string> _listThreadTwo = uniqueGames.Skip(count ).Take(count).ToList();
            List<string> _listThreadThree = uniqueGames.Skip(count * 2).Take(count).ToList();
            List<string> _listThreadFour = uniqueGames.Skip(count * 3).Take(count).ToList();
            List<string> _listThreadFive = uniqueGames.Skip(count * 4).Take(count).ToList();
            List<string> _listThreadSix = uniqueGames.Skip(count * 5).Take(count).ToList();
            List<string> _listThreadSeven = uniqueGames.Skip(count * 6).Take(count).ToList();
            List<string> _listThreadEight = gamesId.Skip(count * 7).Take(count).ToList();
            List<string> _listThreadNine = uniqueGames.Skip(count * 8).Take(count).ToList();
            List<string> _listThreadTen = uniqueGames.Skip(count * 9).Take(count).ToList();
            List<string> _listThreadEleven = uniqueGames.Skip(count * 10).Take(count).ToList();
            List<string> _listThreadTwelve = uniqueGames.Skip(count * 11).Take(count).ToList();
            List<string> _listThreadThirteen = uniqueGames.Skip(count * 12).Take(count).ToList();
            List<string> _listThreadFourteen = uniqueGames.Skip(count * 13).Take(count).ToList();
            List<string> _listThreadFifteen = uniqueGames.Skip(count * 13).Take(count).ToList();

            Thread thread = new(() =>
            {
                ReadGameInfo(_listThreadOne,1 );
                Console.WriteLine("Thread 1 started");
            });
            Thread threadTwo = new(() =>
            {
                ReadGameInfo(_listThreadTwo,2 );
                Console.WriteLine("Thread 2 started");
            });
            Thread threadThree = new(() =>
            {
                ReadGameInfo(_listThreadThree, 3);
                Console.WriteLine("Thread 3 started");
            });

            Thread threadFour = new(() =>
            {
                ReadGameInfo(_listThreadFour, 4);
                Console.WriteLine("Thread 4 started");
            });

            Thread threadFive = new(() =>
            {
                ReadGameInfo(_listThreadFive, 5);
                Console.WriteLine("Thread 5 started");
            });

            Thread threadSix = new(() =>
            {
                ReadGameInfo(_listThreadSix, 6);
                Console.WriteLine("Thread 6 started");
            });

            Thread threadSeven = new(() =>
            {
                ReadGameInfo(_listThreadSeven, 7);
                Console.WriteLine("Thread 7 started");
            });

            Thread threadEight = new(() =>
            {
                ReadGameInfo(_listThreadEight, 8);
                Console.WriteLine("Thread 8 started");
            });

            Thread threadNine = new(() =>
            {
                ReadGameInfo(_listThreadNine, 9);
                Console.WriteLine("Thread 9 started");
            });

            Thread threadTen = new(() =>
            {
                ReadGameInfo(_listThreadTen, 10);
                Console.WriteLine("Thread 10 started");
            });

            Thread threadEleven = new(() =>
            {
                ReadGameInfo(_listThreadEleven, 11);
                Console.WriteLine("Thread 11 started");
            });
            
            Thread threadTwelve = new(() =>
            {
                ReadGameInfo(_listThreadTwelve, 12);
                Console.WriteLine("Thread 12 started");
            });

            Thread threadThirteen = new(() =>
            {
                ReadGameInfo(_listThreadThirteen, 13);
                Console.WriteLine("Thread 13 started");
            });

            Thread threadFourteen = new(() =>
            {
                ReadGameInfo(_listThreadFourteen, 14);
                Console.WriteLine("Thread 14 started");
            });

            Thread threadFifteen = new(() =>
            {
                ReadGameInfo(_listThreadFifteen, 15);
                Console.WriteLine("Thread 15 started");
            });

            thread.Start();
            threadTwo.Start();
            threadThree.Start();
            threadFour.Start();
            threadFive.Start();
            threadSix.Start();
            threadSeven.Start();
            threadEight.Start();
            threadNine.Start();
            threadTen.Start();
            threadEleven.Start();
            threadTwelve.Start();
            threadThirteen.Start();
            threadFourteen.Start();
            threadFifteen.Start();

            Console.WriteLine("All threads started");
            countdownEvent.Wait();
            Console.WriteLine("Write game data in google sheets...");
            google.WriteEntries(_gamesInfo);
            await req.SendTelegram($"Table entry completed");
            Console.WriteLine("Table entry completed");
        }


        private static async void ReadGameInfo(List<string> gameList, int thread)
        {
            Helpers helper = new();
            HtmlDocument htmlDoc = new();
            GetNode node = new();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"parser/log.txt") 
                .CreateLogger();

            int res = 0;
            string gameInfo = string.Empty;

            Proxy? proxyConfig = JsonConvert.DeserializeObject<Proxy>(File.ReadAllText($@"/root/parse/Config/proxy.json"));
            Uri proxyUri = new(proxyConfig.ProxyUri);
            NetworkCredential proxyCredentials = new(proxyConfig.ProxyLogin, proxyConfig.ProxyPassword);

            HttpClientHandler httpClientHandler = new()
            {
                Proxy = new WebProxy(proxyUri)
                {
                    Credentials = proxyCredentials,
                },
                UseCookies = true,
                UseDefaultCredentials = true,
                UseProxy = true,
            };
            HttpClient proxyClient = new(httpClientHandler);

            try {
                foreach (string game in gameList)
                {
                    string htmlCode = await proxyClient.GetStringAsync($"https://store.playstation.com/en-tr/product/{game}");

                    htmlDoc.LoadHtml(htmlCode);
                    HtmlNode gameTitle = htmlDoc.DocumentNode.SelectSingleNode("//h1[contains(@class,'psw-m-b-5 psw-t-title-l')]");
                    HtmlNode priceFirstLine = htmlDoc.DocumentNode.SelectSingleNode("(//div[contains(@class,'psw-l-anchor psw-l-stack-left')]//span)[1]");
                    HtmlNode priceSecondLine = htmlDoc.DocumentNode.SelectSingleNode("(//span[contains(@class,'psw-fill-x psw-l-line-left')])[2]");
                    HtmlNode editionLine = htmlDoc.DocumentNode.SelectSingleNode("//span[@data-qa='mfe-game-title#productTag1']");
                    HtmlNode voiceLine = htmlDoc.DocumentNode.SelectSingleNode("//dd[@data-qa='gameInfo#releaseInformation#voice-value']");
                    HtmlNode subtitlesLine = htmlDoc.DocumentNode.SelectSingleNode("//dd[@data-qa='gameInfo#releaseInformation#subtitles-value']");
                    HtmlNode releaseLine = htmlDoc.DocumentNode.SelectSingleNode("(//dd[contains(@class,'psw-p-r-6 psw-p-r-0@tablet-s')])[2]");
                    HtmlNode discountOneLine = htmlDoc.DocumentNode.SelectSingleNode("//span[@data-qa='mfeCtaMain#offer0#discountInfo']");
                    HtmlNode discountOneEndLine = htmlDoc.DocumentNode.SelectSingleNode("//span[@data-qa='mfeCtaMain#offer0#discountDescriptor']");
                    HtmlNode discountTwoLine = htmlDoc.DocumentNode.SelectSingleNode("//span[contains(@class,'psw-t-overline psw-t-bold')]");
                    HtmlNode versionLine = htmlDoc.DocumentNode.SelectSingleNode("//dd[@data-qa='gameInfo#releaseInformation#platform-value']");

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

                    string[] priceres = helper.SplitFirstPrice(priceOne);
                    gameInfo = $"{game} | {headerText}";
                    Console.WriteLine($"Thread {thread} position {res++}/{gameList.Count} | {game}:{headerText}");
                    Log.Information($"{game}:{headerText}");
                    node.sendGameData(game, headerText?.Replace("%amp;", ""), version, priceres, helper.slitSecondPrice(priceTwo),
                        helper.editionEdit(edition), voice, subtitles, release, 
                        discountOne, discountOneEnd, helper.discountTwoTrim(discountTwo));
                }
                Log.CloseAndFlush();
            } 
            catch (Exception ex) {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.File(@"parser/exc.txt")
                    .CreateLogger();
                Console.WriteLine("exc " + ex.StackTrace + $"\n {ex.Message}");
                Log.Error("exc " + ex.StackTrace + $"\n {ex.Message} | {gameInfo}");
                Log.CloseAndFlush();
            }
            finally
            {
                countdownEvent.Signal();
            }

        }

        private void sendGameData(string game, string title, string version, string[] priceOne, string[] priceTwo, string edition, string voice, 
            string subtitles, string release, string discountOne, string discountOneEnd, string discountTwo)
        {
            try
            {
                lock (lockObject)
                {
                    _gamesInfo.Add(new List<object> { game ?? "", $"https://store.playstation.com/en-tr/product/{game}/", title ?? "", version ?? "", edition ?? "", priceOne[0], priceOne[1], discountOne ?? "", discountOneEnd ?? "", priceTwo[0], priceTwo[1], discountTwo ?? "", "", release ?? "", "url", voice ?? "", subtitles ?? "" });
                }
            }
            catch (Exception e) { Console.WriteLine("exc " + e.Message + "\n" + e.StackTrace); }
        }
    }
}
