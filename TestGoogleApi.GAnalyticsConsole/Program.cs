using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TestGoogleApi.GAnalyticsConsole
{
    /// <summary>
    /// https://www.daimto.com/googleAnalytics-authentication-csharp
    /// https://www.c-sharpcorner.com/UploadFile/f9f215/how-to-fetch-google-analytics-statistics-and-display-it-in-y/
    /// </summary>
    class Program
    {
        static string[] scopes = new string[] {
        AnalyticsService.Scope.Analytics,               // view and manage your Google Analytics data
        AnalyticsService.Scope.AnalyticsEdit,           // Edit and manage Google Analytics Account
        AnalyticsService.Scope.AnalyticsManageUsers,    // Edit and manage Google Analytics Users
        AnalyticsService.Scope.AnalyticsReadonly};      // View Google Analytics Data
        static string ApplicationName = "Analytics API Sample";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            UserCredential credential;

            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                Console.WriteLine("Credential file saved to: " + credPath);
            }

            var service = new AnalyticsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var websiteCode = "67960695";
            var request = service.Data.Ga.Get(
               "ga:" + websiteCode,
               DateTime.Today.AddDays(-15).ToString("yyyy-MM-dd"),
               DateTime.Today.ToString("yyyy-MM-dd"),
               "ga:sessions");
            request.Dimensions = "ga:year,ga:month,ga:day";
            var data = request.Execute();

            foreach (var row in data.Rows)
            {
                visitsData.Add(new ChartRecord(new DateTime(int.Parse(row[0]), int.Parse(row[1]), int.Parse(row[2])).ToString("MM-dd-yyyy"), int.Parse(row[3])));
            }
        }

        static private List<ChartRecord> visitsData = new List<ChartRecord>();

        class ChartRecord
        {
            public ChartRecord(string date, int visits)
            {
                _date = date;
                _visits = visits;
            }
            private string _date;
            public string Date
            {
                get { return _date; }
                set { _date = value; }
            }
            private int _visits;
            public int Visits
            {
                get { return _visits; }
                set { _visits = value; }
            }
        }
    }
}