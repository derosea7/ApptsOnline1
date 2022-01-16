using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Appts.Dal.RedisCache;
using Appts.Messaging.ServiceBus;
using Appts.Web.Ui.Scheduler.Authorization;
using Appts.Web.Ui.Scheduler.Middleware;
using Appts.Web.Ui.Scheduler.Repositories;
using Appts.Web.Ui.Scheduler.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace Appts.Web.Ui.Scheduler
{
  public class Startup
  {
    private readonly ILogger _logger;
    public Startup(IConfiguration configuration, ILogger<Startup> logger)
    {
      Configuration = configuration;
      _logger = logger;
    }
    public IConfiguration Configuration { get; }
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      //services.AddHttpsRedirection(options =>
      //{
      //  options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
      //  options.HttpsPort = 443;
      //});
      //key to working ubuntu 20.4 linux docker host
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
      services.AddHttpClient<IApiClient, ApiClient>();
      services.AddHttpClient<IGoogleCalendarClient, GoogleCalendarClient>();
      services.AddSingleton<IRedisConnectionFactory>(
        r => new RedisConnectionFactory(Configuration["AzureCacheForRedis:Password"]));
      services.AddHttpContextAccessor();
      services.AddTransient<IHttpContextResolverService, HttpContextResolverService>();
      services.AddTransient<IOrganizationRepository, OrganizationRepository>();
      services.AddTransient<IServiceProviderRepository, ServiceProviderRepository>();
      services.AddTransient<ITokenCacheRepository, TokenCacheRepository>();
      services.AddTransient<IAppointmentRepository, AppointmentRepository>();
      services.AddTransient<IBlobAvatarRepository, BlobAvatarRepository>();
      services.AddTransient<IBus, Bus>();
      services.AddTransient<IApptTelemetryRepository, ApptTelemetryRepository>();
      services.AddScoped<IAuthorizationHandler, SchedulingPrivacyLevelHandler>();
      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });
      // policies
      services.AddAuthorization(options =>
      {
        options.AddPolicy("PaidSubscriber", policyBuilder =>
        {
          policyBuilder.RequireAuthenticatedUser();
          policyBuilder.RequireClaim("Type", "Sp");
        });
        options.AddPolicy("ApptsAdmin", policyBuilder =>
        {
          policyBuilder.RequireAuthenticatedUser();
          policyBuilder.RequireClaim("Type", "Admin");
        });
        //options.AddPolicy("Client", policyBuilder =>
        //{
        //  policyBuilder.RequireAuthenticatedUser();
        //  policyBuilder.RequireClaim("ext_isSub", "false");
        //});
        //options.AddPolicy("AnonymousScheduler", policyBuilder =>
        //{
        //  policyBuilder.AddRequirements(new SpMustAllowAnonymousRequirement());
        //});
        options.AddPolicy("SchedulingPrivacy", policyBuilder =>
        {
          policyBuilder.AddRequirements(new ScheduleAppointmentRequirement());
        });
      });
      services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme)
        .AddAzureADB2C(options => Configuration.Bind("AzureAdB2C", options));
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
      services.Configure<OpenIdConnectOptions>(AzureADB2CDefaults.OpenIdScheme, options =>
      {
        options.Events = new OpenIdConnectEvents()
        {
          OnTokenValidated = SecurityTokenValidated,
          OnAuthenticationFailed = AuthenticationFailed
          , OnRemoteFailure = OnRemoteFailure
        };
      });
      services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTSCONNECTIONSTRING"]);
    }
    private Task OnRemoteFailure(RemoteFailureContext context)
    {
      if (!string.IsNullOrEmpty(context.Failure.Message) &&
        context.Failure.Message.Contains("access_denied") &&
        context.Failure.Message.Contains("AADB2C90118"))
      {
        //context.Response.Redirect($"/Account/PasswordReset?ReturnUrl={context.HttpContext.Items["redirect_uri"]}");
        context.Response.Redirect("/Account/PasswordReset");
        context.HandleResponse();
      }
      //else if (!string.IsNullOrEmpty(context.Failure.Message) &&
      //  context.Failure.Message.Contains("server_error") &&
      //  context.Failure.Message.Contains("AADB2C99001"))
      //{
      //  context.Response.Redirect("/Account/AlreadyExists");
      //  context.HandleResponse();
      //}
        return Task.CompletedTask;
    }
    private Task AuthenticationFailed(Microsoft.AspNetCore.Authentication.OpenIdConnect.AuthenticationFailedContext notification)
    {
      notification.HandleResponse();
      //var af = new Microsoft.AspNetCore.Authentication.OpenIdConnect.AuthenticationFailedContext()
      //  { 
      //ProtocolMessage
      //}
      if (notification.ProtocolMessage.ErrorDescription != null && notification.ProtocolMessage.ErrorDescription.Contains("AADB2C90118"))
      {
        // If the user clicked the reset password link, redirect to the reset password route
        notification.Response.Redirect("/Account/ResetPassword");
      }
      else if (notification.Exception.Message == "access_denied")
      {
        // If the user canceled the sign in, redirect back to the home page
        notification.Response.Redirect("/");
      }
      else
      {
        notification.Response.Redirect("/Home/Error?message=" + notification.Exception.Message);
      }
      return Task.FromResult(0);
    }
    private Task SecurityTokenValidated(TokenValidatedContext context)
    {
      return Task.Run(async () =>
      {
        var orgIdCsv = context.SecurityToken.Claims.FirstOrDefault(c => c.Type == "ext_orgId").Value;
        var ids = orgIdCsv.Split(',');
        foreach (string id in ids)
        {
          var c = id.Split('=');
          //((ClaimsIdentity)context.Principal.Identity).AddClaim(new Claim(ClaimTypes.Role, c[0], c[1]));
          ((ClaimsIdentity)context.Principal.Identity).AddClaim(new Claim(c[0], c[1]));
          //switch (id)
          //{
          //  case "ApptsAdmin":
          //    ((ClaimsIdentity)context.Principal.Identity).AddClaim(new Claim(ClaimTypes.Role, "ApptsAdmin", "ApptsAdmin"));
          //    break;

          //  default:
          //    break;
          //}
        }
      });
    }
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      //key to working ubuntu 20.4 linux docker host
      app.UseForwardedHeaders();
      //app.UseForwardedHeaders(new ForwardedHeadersOptions
      //{
      //  ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      //});
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        //app.UseDeveloperExceptionPage();
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }
      app.ConfigureCustomExceptionMiddleware();
      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseCookiePolicy();
      app.UseAuthentication();
      new OpenIdConnectOptions()
      {
        Events = new OpenIdConnectEvents()
        {
          OnTokenValidated = SecurityTokenValidated
        }
      };
      //app.Use(async (ctx, next) =>
      //{
      //  await next();
      //  if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
      //  {
      //    //re-execute the request so the user gets the error page
      //    string originalPath = ctx.Request.Path.Value;
      //    ctx.Items["originalPath"] = originalPath;
      //    ctx.Request.Path = "/error/404";
      //    await next();
      //  }
      //});
      // some code to change signout landing page, not using yet
      //app.UseRewriter(new RewriteOptions().Add(context =>
      //{
      //  if (context.HttpContext.Request.Path == "/AzureADB2C/Account/SignedOut")
      //  {
      //    context.HttpContext.Response.Redirect("/Home/SignedOut");
      //  }
      //}));
      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}