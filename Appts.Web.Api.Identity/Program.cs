using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Logging;

namespace Appts.Web.Api.Identity
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
        logging.AddApplicationInsights("baaa38dc-1d34-4e06-9181-4eefd3b58a72");
      })
      .ConfigureAppConfiguration((context, config) =>
      {

        if (context.HostingEnvironment.IsProduction())
        //if (context.HostingEnvironment.IsDevelopment())
        {
          var builtConfig = config.Build();

          //var azureServiceTokenProvider = new AzureServiceTokenProvider();
          //var keyVaultClient = new KeyVaultClient(
          //  new KeyVaultClient.AuthenticationCallback(
          //    azureServiceTokenProvider.KeyVaultTokenCallback));

          //config.AddAzureKeyVault(
          //  $"https://{builtConfig["KeyVault:Name"]}.vault.azure.net/",
          //  keyVaultClient,
          //  new DefaultKeyVaultSecretManager());
          config.AddAzureKeyVault(
            $"https://{builtConfig["KeyVault:Name"]}.vault.azure.net/",
            builtConfig["KeyVault:SpClientId"], builtConfig["KeyVault:SpSecret"]);
        }
        else if (context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsStaging())
        {
          config.AddUserSecrets("07ea42f0-be66-4bae-8b13-f5b0038f13cb");
        }

      })
      .UseStartup<Startup>();
  }
}
