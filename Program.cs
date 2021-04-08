
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoginService
{
    public class Program
    {
        /*  static string[] Scopes = { GmailService.Scope.GmailSend };
         static string ApplicationName = "Gmail API .NET Quickstart";
  */
        public static void Main(string[] args)
        {


            /* 

                        UserCredential credential;

                        using (var stream =
                            new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                        {
                            // The file token.json stores the user's access and refresh tokens, and is created
                            // automatically when the authorization flow completes for the first time.
                            string credPath = "token.json";
                            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                                GoogleClientSecrets.Load(stream).Secrets,
                                Scopes,
                                "user",
                                CancellationToken.None,
                                new FileDataStore(credPath, true)).Result;
                            Console.WriteLine("Credential file saved to: " + credPath);
                        }

                        // Create Gmail API service.
                        var service = new GmailService(new BaseClientService.Initializer()
                        {
                            HttpClientInitializer = credential,
                            ApplicationName = ApplicationName,
                        });

                        // Define parameters of request.
                        UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");

             */


            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    }
}
