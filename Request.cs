namespace PS_parser
{
    internal class Request
    {
        public async Task SendTelegram(string message)
        {
            string botToken = "5896727531:AAHC6s57hwE22cZD2uWLT6RqGYnmp_qb2g8";
            string chatId = "5394035773";
            using HttpClient client = new();
            string apiUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";

            FormUrlEncodedContent content = new(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId.ToString()),
                new KeyValuePair<string, string>("text", message)
            });

            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Telegram message sent success");
            }
            else
            {
                Console.WriteLine("Telegram error sending. Status code: " + response.StatusCode);
            }
        }
    }
}
