using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PS_parser
{
    internal class ParseGameId
    {

        private HtmlNodeCollection _nodes;
        private List<string> _gamesId = new();

        public async Task<List<string>> GetGamesRequest()
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
                Console.WriteLine($"PS 5 parsed: {i}/{maxPagePS5} page");
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
                Console.WriteLine($"PS4 parsed: {i}/{maxPage} page");
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
            return _gamesId;
        }
    }
}
