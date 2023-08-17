using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace PS_parser.Sheets
{
    internal class SheetsApi
    {
        static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static readonly string ApplicationName = "Playstation";
        static readonly string SpreadSheetId = "1TJrEj_lpb5K91HRZr0uofa6FgD2GiP0KI14WJYFFDoA";
        static readonly string Sheet = "Лист1";
        static SheetsService Service;

        private void Auth()
        {
            GoogleCredential credential;
            using var stream = new FileStream("google.json", FileMode.Open, FileAccess.Read);
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);

            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });
        }
        public void ReadEntries(IList<IList<object>> values)
        {
            Auth();
            try
            {
                ClearValuesRequest clearRequest = new();
                var range = $"{Sheet}!A3:Q20000";
                var requestDelete = Service.Spreadsheets.Values.Clear(clearRequest, SpreadSheetId, range);
                requestDelete.Execute();
                ValueRange valueRange = new()
                {
                    Values = values
                };
                var request =
                    Service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
                request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
                request.Execute();
                /*var range = $"{Sheet}!A1:H7";
                var request = service.Spreadsheets.Values.Get(SpreadSheetId, range);
                var response = request.Execute();
                var values = response.Values;
                foreach (var entry in values)
                {
                    Console.WriteLine($"{entry[1]}");
                }*/
            }
            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
        }

    }
}
