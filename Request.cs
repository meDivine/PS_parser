using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace PS_parser
{
    internal class Request
    {


        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Здесь вы можете реализовать вашу логику проверки SSL сертификатов.
            // Возвращение true означает, что SSL ошибки будут игнорироваться.
            return true;
        }


        public async Task<string> GetRequestGameInfo(string game)
        {
            using var client = new HttpClient();
            string url = $"https://store.playstation.com/en-tr/product/{game}";
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
            var response = await client.GetAsync(url);
            string content = string.Empty;
            try
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    content = await response.Content.ReadAsStringAsync();
                }
                else if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    var proxyUri = new Uri("https://91.188.243.194:9498"); // Замените на адрес вашего HTTPS прокси
                    var proxyCredentials = new NetworkCredential("3cjHxg", "bgGRsn"); // Замените на свои учетные данные

                    var httpClientHandler = new HttpClientHandler
                    {
                        Proxy = new WebProxy(proxyUri)
                        {
                            Credentials = proxyCredentials
                        },
                        UseProxy = true,
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return content;
        }
        
    }
}
