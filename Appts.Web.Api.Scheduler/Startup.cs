using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Appts.Dal.Cosmos;
using Appts.Web.Api.Scheduler.Repositories;
using Appts.Messaging.ServiceBus;
using Microsoft.Extensions.Logging.AzureAppServices;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http;
using Appts.Web.Api.Scheduler.Services;

//tmp to view cliams before tranformation
//using System.IdentityModel.Tokens.Jwt;

namespace Appts.Web.Api.Scheduler
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;

      // clears claims transformations
      //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<ForwardedHeadersOptions>(options =>
      {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto;
        // Only loopback proxies are allowed by default.
        // Clear that restriction because forwarders are enabled by explicit 
        // configuration.
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();
      });
      services.AddSingleton<IDb>(
        options =>
        {
          var databaseId = Configuration["CosmosDb:DatabaseId"];
          var containerId = Configuration["CosmosDb:SpContainer:ContainerId"];
          var paritionKey = Configuration["CosmosDb:SpContainer:PartitionKey"];
          var endpoint = Configuration["CosmosDb:EndpointUri"];
          var primaryKey = Configuration["CosmosDb:PrimaryKey"];
          return new Db(databaseId, containerId, endpoint, primaryKey, paritionKey);
        });

      services.AddSingleton<IUserDb>(
        options =>
        {
          var databaseId = Configuration["CosmosDb:DatabaseId"];
          var containerId = Configuration["CosmosDb:UsersContainer:ContainerId"];
          var partitionKey = Configuration["CosmosDb:UsersContainer:PartitionKey"];
          var endpoint = Configuration["CosmosDb:EndpointUri"];
          var primaryKey = Configuration["CosmosDb:PrimaryKey"];
          return new UserDb(databaseId, containerId, endpoint, primaryKey, partitionKey);
        });
      //services.AddTransient<IDb, Db>(options =>
      //{
      //  var databaseId = Configuration["CosmosDb:DatabaseId"];
      //  var containerId = Configuration["CosmosDb:ContainerId"];
      //  var endpoint = Configuration["CosmosDb:EndpointUri"];
      //  var primaryKey = Configuration["CosmosDb:PrimaryKey"];
      //  return new Db(databaseId, containerId, endpoint, primaryKey);
      //});
      services.AddTransient<IOrganizationRepository, OrganizationRepository>();
      services.AddTransient<IServiceProviderRepository, ServiceProviderRepository>();
      services.AddTransient<IAvailabilityRepository, AvailabilityRepository>();
      services.AddTransient<IAppointmentRepository, AppointmentRepository>();
      services.AddTransient<IAppointmentTypeRepository, AppointmentTypeRepository>();
      services.AddTransient<ITokenCacheRepository, TokenCacheRepository>();
      services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();
      services.AddTransient<ILookupRepository, LookupRepository>();
      services.AddTransient<IClientRepository, ClientRepository>();
      services.AddTransient<IUserRepository, UserRepository>();
      services.AddSingleton<IBus, Bus>();
      //  options => 
      //  {
      //    var conn = new ServiceBusConnection(Configuration["ServiceBus:ConnectionString"].ToString());
      //    return new Bus(conn);
      //});
      services.AddTransient<ICommunicationService, CommunicationService>();
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.Audience = Configuration["AzureAdB2C:ClientId"];
          options.Authority = "https://login.microsoftonline.com/7ed000de-2ad4-4031-9917-4fbc9f12c405/v2.0";
        });
      //services.Configure<AzureBlobLoggerOptions>(Configuration.GetSection("AzureLogging"));
      services.Configure<AzureBlobLoggerOptions>(Configuration.GetSection("AzureLogging"));
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTSCONNECTIONSTRING"]);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app.UseForwardedHeaders();
      //app.Run(async (context) =>
      //{
      //  context.Response.ContentType = "text/plain";

      //  // Request method, scheme, and path
      //  await context.Response.WriteAsync(
      //      $"Request Method: {context.Request.Method}{Environment.NewLine}");
      //  await context.Response.WriteAsync(
      //      $"Request Scheme: {context.Request.Scheme}{Environment.NewLine}");
      //  await context.Response.WriteAsync(
      //      $"Request Path: {context.Request.Path}{Environment.NewLine}");

      //  // Headers
      //  await context.Response.WriteAsync($"Request Headers:{Environment.NewLine}");

      //  foreach (var header in context.Request.Headers)
      //  {
      //    await context.Response.WriteAsync($"{header.Key}: " +
      //        $"{header.Value}{Environment.NewLine}");
      //  }

      //  await context.Response.WriteAsync(Environment.NewLine);

      //  // Connection: RemoteIp
      //  await context.Response.WriteAsync(
      //      $"Request RemoteIp: {context.Connection.RemoteIpAddress}");
      //});
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseAuthentication();
      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "api/{controller}/{action}/{id?}");
      });
    }
  }
}
