using HtmlAgilityPack;
using PS_parser.Sheets;
using Serilog;
using System.Text.RegularExpressions;

namespace PS_parser
{
    internal class GetNode
    {
        static IList<IList<object>> _gamesInfo = new List<IList<object>>();
        static object lockObject = new object();
        static int threadCount = 15;
        static CountdownEvent countdownEvent = new(threadCount);

        public async Task StartThreadsPS5()
        {
            var GetGames = new ParseGameId();
            var gamesId = await GetGames.GetGamesRequest();

            int count = gamesId.Count / 15;

            var _listThreadOne = gamesId.Take(count).ToList();
            var _listThreadTwo = gamesId.Skip(count ).Take(count).ToList();
            var _listThreadThree = gamesId.Skip(count * 2).Take(count).ToList();
            var _listThreadFour = gamesId.Skip(count * 3).Take(count).ToList();
            var _listThreadFive = gamesId.Skip(count * 4).Take(count).ToList();
            var _listThreadSix = gamesId.Skip(count * 5).Take(count).ToList();
            var _listThreadSeven = gamesId.Skip(count * 6).Take(count).ToList();
            var _listThreadEight = gamesId.Skip(count * 7).Take(count).ToList();
            var _listThreadNine = gamesId.Skip(count * 8).Take(count).ToList();
            var _listThreadTen = gamesId.Skip(count * 9).Take(count).ToList();
            var _listThreadEleven = gamesId.Skip(count * 10).Take(count).ToList();
            var _listThreadTwelve = gamesId.Skip(count * 11).Take(count).ToList();
            var _listThreadThirteen = gamesId.Skip(count * 12).Take(count).ToList();
            var _listThreadFourteen = gamesId.Skip(count * 13).Take(count).ToList();
            var _listThreadFifteen = gamesId.Skip(count * 13).Take(count).ToList();

            var google = new SheetsApi();

            var thread = new Thread(() =>
            {
                ReadGameInfo(_listThreadOne,1 );
                Console.WriteLine("Thread 1 started");
            });
            var threadTwo = new Thread(() =>
            {
                ReadGameInfo(_listThreadTwo,2 );
                Console.WriteLine("Thread 2 started");
            });
            var threadThree = new Thread(() =>
            {
                ReadGameInfo(_listThreadThree, 3);
                Console.WriteLine("Thread 3 started");
            });

            var threadFour = new Thread(() =>
            {
                ReadGameInfo(_listThreadFour, 4);
                Console.WriteLine("Thread 4 started");
            });

            var threadFive = new Thread(() =>
            {
                ReadGameInfo(_listThreadFive, 5);
                Console.WriteLine("Thread 5 started");
            });

            var threadSix = new Thread(() =>
            {
                ReadGameInfo(_listThreadSix, 6);
                Console.WriteLine("Thread 6 started");
            });

            var threadSeven = new Thread(() =>
            {
                ReadGameInfo(_listThreadSeven, 7);
                Console.WriteLine("Thread 7 started");
            });

            var threadEight = new Thread(() =>
            {
                ReadGameInfo(_listThreadEight, 8);
                Console.WriteLine("Thread 8 started");
            });

            var threadNine = new Thread(() =>
            {
                ReadGameInfo(_listThreadNine, 9);
                Console.WriteLine("Thread 9 started");
            });

            var threadTen = new Thread(() =>
            {
                ReadGameInfo(_listThreadTen, 10);
                Console.WriteLine("Thread 10 started");
            });

            var threadEleven = new Thread(() =>
            {
                ReadGameInfo(_listThreadEleven, 11);
                Console.WriteLine("Thread 11 started");
            });
            
            var threadTwelve = new Thread(() =>
            {
                ReadGameInfo(_listThreadTwelve, 12);
                Console.WriteLine("Thread 12 started");
            });

            var threadThirteen = new Thread(() =>
            {
                ReadGameInfo(_listThreadThirteen, 13);
                Console.WriteLine("Thread 13 started");
            });

            var threadFourteen = new Thread(() =>
            {
                ReadGameInfo(_listThreadFourteen, 14);
                Console.WriteLine("Thread 14 started");
            });

            var threadFifteen = new Thread(() =>
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
            google.ReadEntries(_gamesInfo);
            Console.WriteLine("Table entry completed");
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
            string gameInfo;

            try {
                foreach (var game in gameList)
                {   
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
                    var priceOne = priceFirstLine?.InnerText;
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
                    .WriteTo.File("exc.txt")
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
