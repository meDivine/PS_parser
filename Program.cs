using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


List<string> tags = new List<string>();

async Task parse()
{
    for (int i = 1; i <= 1; i++)
    {
        using var client = new HttpClient();
        string htmlCode = await client.GetStringAsync($"https://store.playstation.com/en-tr/pages/browse/{i}");
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlCode);
        var headerNodes = htmlDoc.DocumentNode.SelectNodes("//ul[@class='psw-grid-list psw-l-grid']");

        if (headerNodes != null)
        {
            foreach (var headerNode in headerNodes)
            {
                string headerText = headerNode.InnerHtml;

                string pattern = @"href=""/en-tr/concept/(.*?)""";
                MatchCollection matches = Regex.Matches(headerText, pattern);

                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 1)
                    {
                        string hrefValue = match.Groups[1].Value;
                        tags.Add(hrefValue);
                    }
                }
            }
        }
        Console.WriteLine("Спарсил стриницу:" + i);
    }
}

await parse();

foreach(var tag in tags)
{
    using var client = new HttpClient();
    string htmlCode = await client.GetStringAsync($"https://store.playstation.com/en-tr/concept/{tag}");
    var htmlDoc = new HtmlDocument();
    htmlDoc.LoadHtml(htmlCode);
    var headerNodes = htmlDoc.DocumentNode.SelectNodes("(//div[contains(@class,'psw-l-anchor psw-l-stack-left')])[2]");

    if (headerNodes != null)
    {
        foreach (var headerNode in headerNodes)
        {
            string headerText = headerNode.InnerText;
            Console.WriteLine(headerText);
        }
    }
}