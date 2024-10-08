﻿using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Serilog;

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
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"parser/exc.txt")
                .CreateLogger();
            try
            {
                GoogleCredential credential;
                using FileStream stream = new($@"Config/google.json", FileMode.Open, FileAccess.Read);
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);

                Service = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Log.Error("exc " + ex.StackTrace + $"\n {ex.Message}");
                Log.CloseAndFlush();
            }
        }
        public void WriteEntries(IList<IList<object>> values)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"parser/exc.txt")
                .CreateLogger();
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
                WriteDateTime();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Log.Error("exc " + ex.StackTrace + $"\n {ex.Message}");
                Log.CloseAndFlush();
            }
        }

        private void WriteDateTime()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(@"parser/exc.txt")
                .CreateLogger();
            Auth();
            try
            {
                TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");
                DateTime moscowTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, moscowTimeZone);
                ClearValuesRequest clearRequest = new();
                string range = $"{Sheet}!A1";
                SpreadsheetsResource.ValuesResource.ClearRequest requestDelete = Service.Spreadsheets.Values.Clear(clearRequest, SpreadSheetId, range);
                requestDelete.Execute();

                List<IList<object>> values = new()
                {
                    new List<object> { moscowTime.ToString() }
                };
                ValueRange valueRange = new()
                {
                    Values = values
                };
                SpreadsheetsResource.ValuesResource.AppendRequest request =
                    Service.Spreadsheets.Values.Append(valueRange, SpreadSheetId, range);
                request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
                request.Execute();
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.StackTrace);
                Log.Error("exc " + ex.StackTrace + $"\n {ex.Message}");
                Log.CloseAndFlush();
            }
        }

    }
}
