using HtmlAgilityPack;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using PS_parser.Config;

namespace PS_parser
{
    internal class ParseGameId
    {
        private List<string> _gamesId = new();

        public async Task<List<string>> GetGamesRequest()
        {
            Request req = new();
            await req.SendTelegram($"Start parsing PS5/4 Games");

            var proxyClient = CreateHttpClientWithProxy();
            await ParseGamesForPlatform(proxyClient, "PS5", "https://store.playstation.com/en-tr/category/4cbf39e2-5749-4970-ba81-93a489e4570c");
            await ParseGamesForPlatform(proxyClient, "PS4", "https://store.playstation.com/en-tr/category/44d8bb20-653e-431e-8ad0-c0a365f68d2f");

            await req.SendTelegram($"Finish parsing PS5/4 games");
            return _gamesId;
        }

        private HttpClient CreateHttpClientWithProxy()
        {
            Proxy? proxyConfig = JsonConvert.DeserializeObject<Proxy>(File.ReadAllText($@"/root/parse/Config/proxy.json"));

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

            var proxyClient = new HttpClient(httpClientHandler);
            proxyClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");

            return proxyClient;
        }

        private async Task ParseGamesForPlatform(HttpClient client, string platform, string url)
        {
            HtmlDocument htmlDoc = new();
            string pagesContent = await client.GetStringAsync($"{url}/1");
            htmlDoc.LoadHtml(pagesContent);

            HtmlNode parseMaxPages = htmlDoc.DocumentNode.SelectSingleNode("(//span[@class='psw-fill-x '])[5]");
            int maxPages = int.Parse(parseMaxPages.InnerText);
            Console.WriteLine($"Start parsing {platform} Games");

            for (int i = 1; i <= maxPages; i++)
            {
                string pageUrl = $"{url}/{i}";
                await ParsePage(client, pageUrl);
                Console.WriteLine($"{platform} parsed: {i}/{maxPages} page");
            }
        }

        private async Task ParsePage(HttpClient client, string pageUrl)
        {
            HtmlDocument htmlDoc = new();
            string htmlCode = await client.GetStringAsync(pageUrl);
            htmlDoc.LoadHtml(htmlCode);

            var nodes = htmlDoc.DocumentNode.SelectNodes("//ul[@class='psw-grid-list psw-l-grid']");
            if (nodes != null)
            {
                foreach (var headerNode in nodes)
                {
                    ExtractGameIdsFromHtml(headerNode.InnerHtml);
                }
            }
        }

        private void ExtractGameIdsFromHtml(string html)
        {
            string pattern = @"href=""/en-tr/product/(.*?)""";
            var matches = Regex.Matches(html, pattern);

            foreach (Match match in matches)
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
