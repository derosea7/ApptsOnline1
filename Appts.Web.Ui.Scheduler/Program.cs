using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Appts.Web.Ui.Scheduler
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
                .UseApplicationInsights()
            .ConfigureLogging(logging =>
            {
              logging.AddApplicationInsights("ca4123a0-af79-4df1-abe2-eb8596240835");

              // Adding the filter below to ensure logs of all severity from Startup.cs
              // is sent to ApplicationInsights.
              logging.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                               (typeof(Startup).FullName, LogLevel.Trace);
            })
      .ConfigureAppConfiguration((context, config) =>
      {
        if (context.HostingEnvironment.IsProduction())
        //if (context.HostingEnvironment.IsDevelopment())
        {
          var builtConfig = config.Build();

          var azureServiceTokenProvider = new AzureServiceTokenProvider();
          var keyVaultClient = new KeyVaultClient(
            new KeyVaultClient.AuthenticationCallback(
              azureServiceTokenProvider.KeyVaultTokenCallback));

          //config.AddAzureKeyVault(
          //  $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
          //  keyVaultClient,
          //  new DefaultKeyVaultSecretManager());

          config.AddAzureKeyVault(
            $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
            "ed4fe933-3a11-4179-ad7e-34ef7d2d777e", "4-7~pu_ZeWRSZPTt-U50dmDVy~BwR-z6cS");

        }
        else if (context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsStaging())
        {
          config.AddUserSecrets("aspnet-B2C_MVC_UI-373EE415-1105-4701-BABD-42946FF9CC21");
        }
      })
      .UseStartup<Startup>();
  }
}
