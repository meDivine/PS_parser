using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PS_parser
{
    internal class Request
    {

        public async Task<string> GetRequestGameInfo(string game)
        {
            using var client = new HttpClient();
            string url = $"https://store.playstation.com/en-tr/product/{game}";
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
            var response = await client.GetAsync(url);
            string content = string.Empty;

            string[] lines = File.ReadAllLines("proxy.txt");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var random = new Random();
                var randomIndex = random.Next(0, lines.Length);

                var httpClientHandler = new HttpClientHandler
                {
                    Proxy = new WebProxy(lines[randomIndex]),
                    UseProxy = true
                };
                var proxyClient = new HttpClient(httpClientHandler);

                var proxyResponse = await proxyClient.GetAsync(url);

                if (proxyResponse.StatusCode == HttpStatusCode.OK)
                {
                    content = await proxyResponse.Content.ReadAsStringAsync();
                    Console.WriteLine("Response after using proxy:");
                    Console.WriteLine(content);
                }
                else
                {
                    Console.WriteLine("Request failed even with proxy. Status code: " + proxyResponse.StatusCode);
                }
            }
            return content;
        }
    }
}
