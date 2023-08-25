using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PS_parser.Config;

namespace PS_parser
{
    internal class ParseGameId
    {

        private HtmlNodeCollection _nodes;
        private List<string> _gamesId = new();

        public async Task<List<string>> GetGamesRequest()
        {
            using HttpClient client = new();
            HtmlDocument htmlDoc = new();
            Request req = new();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

            Proxy? proxyConfig = JsonConvert.DeserializeObject<Proxy>(File.ReadAllText(@"/root/parse/Config/proxy.json"));

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

            HttpResponseMessage pagesPS5Request = await proxyClient.GetAsync("https://store.playstation.com/en-tr/category/4cbf39e2-5749-4970-ba81-93a489e4570c/1");
            string pagesPS5 = await pagesPS5Request.Content.ReadAsStringAsync();

            htmlDoc.LoadHtml(pagesPS5);
            HtmlNode parseMaxPagesPS5 = htmlDoc.DocumentNode.SelectSingleNode("(//span[@class='psw-fill-x '])[5]");
            string maxPagePS5 = parseMaxPagesPS5.InnerText;
            await req.SendTelegram($"Start parsing PS5 Games");


            for (int i = 1; i <= int.Parse(maxPagePS5); i++)
            {
                HttpResponseMessage proxyResponse = await proxyClient.GetAsync($"https://store.playstation.com/en-tr/category/4cbf39e2-5749-4970-ba81-93a489e4570c/{i}");
                string htmlCode = await proxyResponse.Content.ReadAsStringAsync();
                htmlDoc.LoadHtml(htmlCode);
                _nodes = htmlDoc.DocumentNode.SelectNodes("//ul[@class='psw-grid-list psw-l-grid']");
                Console.WriteLine($"PS5 parsed: {i}/{maxPagePS5} page");

                if (this._nodes != null)
                {
                    foreach (HtmlNode? headerNode in this._nodes)
                    {
                        string headerText = headerNode.InnerHtml;
                        string pattern = @"href=""/en-tr/product/(.*?)""";
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

            HttpResponseMessage pagesPS4Request = await proxyClient.GetAsync($"https://store.playstation.com/en-tr/category/44d8bb20-653e-431e-8ad0-c0a365f68d2f/1");
            string pagesPS4 = await pagesPS4Request.Content.ReadAsStringAsync();
            htmlDoc.LoadHtml(pagesPS4);
            HtmlNode parseMaxPages = htmlDoc.DocumentNode.SelectSingleNode("(//span[@class='psw-fill-x '])[5]");
            string maxPage = parseMaxPages.InnerText;
            await req.SendTelegram($"Start parsing PS4 Games");

            for (int i = 1; i <= int.Parse(maxPage); i++)
            {
                HttpResponseMessage proxyResponse = await proxyClient.GetAsync($"https://store.playstation.com/en-tr/category/44d8bb20-653e-431e-8ad0-c0a365f68d2f/{i}");
                string htmlCode = await proxyResponse.Content.ReadAsStringAsync();
                htmlDoc.LoadHtml(htmlCode);
                _nodes = htmlDoc.DocumentNode.SelectNodes("//ul[@class='psw-grid-list psw-l-grid']");
                Console.WriteLine($"PS4 parsed: {i}/{maxPage} page");

                if (this._nodes != null)
                {
                    foreach (HtmlNode? headerNode in this._nodes)
                    {
                        string headerText = headerNode.InnerHtml;
                        string pattern = @"href=""/en-tr/product/(.*?)""";
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
            await req.SendTelegram($"Finish parsing PS5/4 games");
            return _gamesId;
        }
    }
}
