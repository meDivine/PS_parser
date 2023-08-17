using HtmlAgilityPack;
using PS_parser.Sheets;
using Serilog;
using System.Text.RegularExpressions;

namespace PS_parser
{
    internal class GetNode
    {
        private List<string> _gamesId = new();
        private HtmlNodeCollection _nodes;
        static IList<IList<object>> _gamesInfo = new List<IList<object>>();
        static object lockObject = new object();
        static int threadCount = 5;
        static CountdownEvent countdownEvent = new CountdownEvent(threadCount);

        private async Task GetGamesRequest()
        {
            using var client = new HttpClient();
            var htmlDoc = new HtmlDocument();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

            string pagesPS5 = await client.GetStringAsync($"https://store.playstation.com/en-tr/category/4cbf39e2-5749-4970-ba81-93a489e4570c/1");
            htmlDoc.LoadHtml(pagesPS5);
            var parseMaxPagesPS5 = htmlDoc.DocumentNode.SelectSingleNode("(//span[@class='psw-fill-x '])[5]");
            string maxPagePS5 = parseMaxPagesPS5.InnerText;

            for (int i = 1; i <= int.Parse(maxPagePS5); i++)
            { 
                string htmlCode = await client.GetStringAsync($"https://store.playstation.com/en-tr/category/4cbf39e2-5749-4970-ba81-93a489e4570c/{i}");
               
                htmlDoc.LoadHtml(htmlCode);
                _nodes = htmlDoc.DocumentNode.SelectNodes("//ul[@class='psw-grid-list psw-l-grid']");
                Console.WriteLine($"PS 5 Спарсил стриницу: {i}/{maxPagePS5}");
                if (this._nodes != null)
                {
                    foreach (var headerNode in this._nodes)
                    {
                        var headerText = headerNode.InnerHtml;
                        var pattern = @"href=""/en-tr/product/(.*?)""";
                        MatchCollection matches = Regex.Matches(headerText, pattern);

                        foreach (Match match in matches.Cast<Match>())
                        {
                            if (match.Groups.Count > 1)
                            {
                                string hrefValue = match.Groups[1].Value;
                                _gamesId.Add(hrefValue);
                            }
                        }
                    }
                }
            }

            
            string pagesPS4 = await client.GetStringAsync($"https://store.playstation.com/en-tr/category/44d8bb20-653e-431e-8ad0-c0a365f68d2f/1");
            htmlDoc.LoadHtml(pagesPS4);
            var parseMaxPages = htmlDoc.DocumentNode.SelectSingleNode("(//span[@class='psw-fill-x '])[5]");
            string maxPage = parseMaxPages.InnerText;

            for (int i = 1; i <= int.Parse(maxPage); i++)
            {
                string htmlCode = await client.GetStringAsync($"https://store.playstation.com/en-tr/category/44d8bb20-653e-431e-8ad0-c0a365f68d2f/{i}");

                htmlDoc.LoadHtml(htmlCode);
                _nodes = htmlDoc.DocumentNode.SelectNodes("//ul[@class='psw-grid-list psw-l-grid']");
                Console.WriteLine($"PS4 Спарсил стриницу: {i}/{maxPage}");
                if (this._nodes != null)
                {
                    foreach (var headerNode in this._nodes)
                    {
                        var headerText = headerNode.InnerHtml;
                        var pattern = @"href=""/en-tr/product/(.*?)""";
                        MatchCollection matches = Regex.Matches(headerText, pattern);

                        foreach (Match match in matches.Cast<Match>())
                        {
                            if (match.Groups.Count > 1)
                            {
                                string hrefValue = match.Groups[1].Value;
                                _gamesId.Add(hrefValue);
                            }
                        }
                    }
                }
            }
        }


        public async Task StartThreadsPS5()
        {
            await GetGamesRequest();

            int count = _gamesId.Count / 5;

            var _listThreadOne = _gamesId.Take(count).ToList();
            var _listThreadTwo = _gamesId.Skip(count ).Take(count).ToList();
            var _listThreadThree = _gamesId.Skip(count * 2).Take(count).ToList();
            var _listThreadFour = _gamesId.Skip(count * 3).Take(count).ToList();
            var _listThreadFive = _gamesId.Skip(count * 4).Take(count).ToList();

            var google = new SheetsApi();

            var thread = new Thread(() =>
            {
                ReadGameInfo(_listThreadOne,1 );
                Console.WriteLine("начал первый поток");
            });
            var threadTwo = new Thread(() =>
            {
                ReadGameInfo(_listThreadTwo,2 );
                Console.WriteLine("начал второй поток");
            });
            var threadThree = new Thread(() =>
            {
                ReadGameInfo(_listThreadThree, 3);
                Console.WriteLine("начал третий поток");
            });

            var threadFour = new Thread(() =>
            {
                ReadGameInfo(_listThreadFour, 4);
                Console.WriteLine("начал четвёртый поток");
            });

            var threadFive = new Thread(() =>
            {
                ReadGameInfo(_listThreadFive, 5);
                Console.WriteLine("начал пятый поток");
            });

            thread.Start();
            threadTwo.Start();
            threadThree.Start();
            threadFour.Start();
            threadFive.Start();

            Console.WriteLine("Запустил потоки");
            countdownEvent.Wait();
            Console.WriteLine("Потоки закончились");
            if (thread.ThreadState == ThreadState.Stopped && threadTwo.ThreadState == ThreadState.Stopped && threadThree.ThreadState == ThreadState.Stopped && threadFour.ThreadState == ThreadState.Stopped && threadFive.ThreadState == ThreadState.Stopped)
            {
                Console.WriteLine("All threads have finished. Calling the final method...");
                google.ReadEntries(_gamesInfo);
            }
        }


        private static async void ReadGameInfo(List<string> gameList, int thread)
        {
            var helper = new Helpers();
            var htmlDoc = new HtmlDocument();
            var node = new GetNode();

            var request = new Request();
            Log.Logger = new LoggerConfiguration()
            .WriteTo.File("log.txt") 
            .CreateLogger();
            int res = 0;
            try {
                foreach (var game in gameList)
                {   
                    //string htmlCode = await client.GetStringAsync($"https://store.playstation.com/en-tr/product/EP1194-PPSA13426_00-0708275145281875");
                    var htmlCode = await request.GetRequestGameInfo(game);
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

                    var headerText = gameTitle?.InnerText;
                    var priceOne = priceFirstLine.InnerText;
                    var priceTwo = priceSecondLine?.InnerText;
                    var edition = editionLine?.InnerText;
                    var voice = voiceLine?.InnerText;
                    var subtitles = subtitlesLine?.InnerText;
                    var release = releaseLine?.InnerText;
                    var discountOne = discountOneLine?.InnerText;
                    var discountOneEnd = discountOneEndLine?.InnerText;
                    var discountTwo = discountTwoLine?.InnerText;
                    var version = versionLine?.InnerText;

                    var priceres = helper.splitFirstPrice(priceOne);
                    Console.WriteLine($"Поток {thread} номер {res++}/{gameList.Count} | {game}:{headerText}");
                    Log.Information($"{game}:{headerText}");
                    Log.CloseAndFlush();
                    node.sendGameData(game, headerText.Replace("%amp;", ""), version, priceres, helper.slitSecondPrice(priceTwo),
                        helper.editionEdit(edition), voice, subtitles, release, 
                        discountOne, discountOneEnd, helper.discountTwoTrim(discountTwo));

                }
                countdownEvent.Signal();
                //google.ReadEntries(node._gamesInfo);

            } 
            catch (Exception ex) { 
                Console.WriteLine("exc " + ex.StackTrace + $"\n {ex.Message}");
                Log.Information("exc " + ex.StackTrace + $"\n {ex.Message}");
                Log.CloseAndFlush();
            }
           
        }

        

        private void sendGameData(string game, string title, string version, string[] priceOne, string[] priceTwo, string edition, string voice, 
            string subtitles, string release, string discountOne, string discountOneEnd, string discountTwo)
        {
            try
            {
                lock (lockObject)
                {
                    _gamesInfo.Add(new List<object> { game, $"https://store.playstation.com/en-tr/product/{game}/", title, version, edition, priceOne[0], priceOne[1], discountOne, discountOneEnd, priceTwo[0], priceTwo[1], discountTwo, "", release, "url", voice, subtitles });
                }
            }
            catch (Exception e) { Console.WriteLine("exc " + e.Message + "\n" + e.StackTrace); }
        }
            
    }
}
