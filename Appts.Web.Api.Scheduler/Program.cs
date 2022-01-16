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
namespace Appts.Web.Api.Scheduler
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
      .ConfigureAppConfiguration((context, config) =>
      {
        if (context.HostingEnvironment.IsProduction())
        {
            var builtConfig = config.Build();
          config.AddAzureKeyVault(
            $"https://{builtConfig["KeyVault:Name"]}.vault.azure.net/",
            builtConfig["KeyVault:SpClientId"], builtConfig["KeyVault:SpSecret"]);
        }
          else if (context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsStaging())
          {
            config.AddUserSecrets("aspnet-B2C_API2-EEB23363-8197-4A57-B48F-C8E70BF59F4A");
          }
      })
      .UseStartup<Startup>();
  }
}