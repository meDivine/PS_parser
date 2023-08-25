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
            using FileStream stream = new(@"/root/parse/Config/proxy.json", FileMode.Open, FileAccess.Read);
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
                string range = $"{Sheet}!A3:Q20000";
                SpreadsheetsResource.ValuesResource.ClearRequest requestDelete = Service.Spreadsheets.Values.Clear(clearRequest, SpreadSheetId, range);
                requestDelete.Execute();
                ValueRange valueRange = new()
                {
                    Values = values
                };
                SpreadsheetsResource.ValuesResource.AppendRequest request =
                    Service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
                request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
                request.Execute();
            }
            catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
        }

    }
}
