using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication;
using Appts.Models.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Appts.Common.Constants;
using Appts.Web.Ui.Scheduler.Repositories;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Appts.Web.Ui.Scheduler.ViewModels;
using Appts.Web.Ui.Scheduler.Services;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class AccountController : Controller
  {
    IOptionsMonitor<AzureADB2COptions> _options;
    private readonly IBlobAvatarRepository _blobRepo;
    private readonly ILogger<AccountController> _logger;
    private readonly IHttpContextResolverService _httpContext;
    private TelemetryClient _telemetry;
    public AccountController(
      IOptionsMonitor<AzureADB2COptions> options, 
      IBlobAvatarRepository blobAvatarRepository,
      ILogger<AccountController> logger, 
      IHttpContextResolverService httpContext,
      TelemetryClient telemetry)
    {
      _options = options;
      _blobRepo = blobAvatarRepository;
      _logger = logger;
      _httpContext = httpContext;
      _telemetry = telemetry;
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult SignIn()
    {
      _telemetry.TrackEvent("SignIn");
      ////6/6/2021 - speeding up dev by redirection to page being worked oon
      //return RedirectToAction("Index", "Dashboard");
      string userId = _httpContext.GetUserId();
      if (userId == "")
      { 
        return RedirectToAction("Index", "Stock");
      }
      return RedirectToAction("Index", "Dashboard");
    }
    [HttpGet("[controller]/[action]/{scheme?}")]
    public async Task<IActionResult> SignOut([FromRoute] string scheme)
    {
      _telemetry.TrackEvent("SignOut");
      scheme = scheme ?? AzureADB2CDefaults.AuthenticationScheme;
      var authenticated = await HttpContext.AuthenticateAsync(scheme);
      if (!authenticated.Succeeded)
      {
        return Challenge(scheme);
      }
      var options = _options.Get(scheme);
      string callbackUrl = $"{Request.Scheme}://{Request.Host}/Account/SignedOut";
      return SignOut(
          new AuthenticationProperties { RedirectUri = callbackUrl },
          options.AllSchemes);
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult SignedOut()
    {
      return View();
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult RegisterAsClientOnly()
    {
      _telemetry.TrackEvent("RegisterAsClientOnly");
      return Redirect(GetIefChallengeUrl("B2C_1A_appts_client_su", "ClientAccountCreated"));
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult EditProfile()
    {
      _telemetry.TrackEvent("EditProfile");
      return Redirect(GetIefChallengeUrl("B2C_1A_appts_profile_edit", "EditSuccessful"));
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult EditSuccessful()
    {
      _telemetry.TrackEvent("EditProfileSuccess");
      UserRole role = _httpContext.GetUserRole();
      TempData["EditSuccessful"] = Boolean.TrueString;
      switch (role)
      {
        case UserRole.Client:
          return RedirectToAction("Settings", "Client", new { edit = "success" });

        case UserRole.Subscriber:
          return RedirectToAction("Settings", "ServiceProvider", new { edit = "success" });
      }
      return RedirectToAction("Settings", "Client", new { edit = "success" });
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult TryFree()
    {
      _telemetry.TrackEvent("TryFreeTrialButtonClicked");
      // if a user is logged in, do different things if they happen
      // to click the try free button
      UserRole role = _httpContext.GetUserRole();
      switch (role)
      {
        case UserRole.Client:
          return RedirectToAction("Dashboard", "Client");

        case UserRole.Subscriber:
          return RedirectToAction("Dashboard", "ServiceProvider");
      }
      string policy = "B2C_1A_appts_su_only";
      string redirectAction = "TrialCreated";
      return Redirect(GetIefChallengeUrl(policy, redirectAction));
    }
    public IActionResult AlreadyExists()
    {
      return View();
    }
    private string GetIefChallengeUrl(string policyName, string redirectAction)
    {
      string baseIef = "https://login.microsoftonline.com/scheduler1.onmicrosoft.com/oauth2/v2.0/authorize?";
      return $"{baseIef}p={policyName}&client_id=a04ec536-c68c-4397-b73d-9dd8e172f292&nonce=defaultNonce&redirect_uri=https%3A%2F%2F{this.Request.Host}%2FAccount%2F{redirectAction}&scope=openid&response_type=id_token&prompt=login";
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult ClientAccountCreated()
    {
      _telemetry.TrackEvent("ClientAccountCreated");
      // if successful, update claim
      var identity = User.Identity as ClaimsIdentity;
      //var IsSubscriberClaim = User.Claims.First(e => e.Type == "ext_orgId");
      //identity.RemoveClaim(IsSubscriberClaim);
      identity.AddClaim(new Claim("ext_orgId", "Client"));
      var newPrincipal = new ClaimsPrincipal(identity);

      // cookie contains JWT, JWT contains claims.
      // to update cookie, sign out and back in
      HttpContext.SignOutAsync();
      HttpContext.SignInAsync(newPrincipal);
      return RedirectToAction("Settings", "Client", new { onboarding = "t" });
    }
    // after sign-up, to get the claim attached to identity,
    // sign-out and then back in
    [HttpGet("[controller]/[action]")]
    public IActionResult TrialCreated()
    {
      _telemetry.TrackEvent("ServiceProviderTrialCreated");
      // if successful, update claim
      var identity = User.Identity as ClaimsIdentity;
      //var IsSubscriberClaim = User.Claims.First(e => e.Type == "ext_orgId");
      //identity.RemoveClaim(IsSubscriberClaim);
      identity.AddClaim(new Claim("ext_orgId", "Paid"));
      var newPrincipal = new ClaimsPrincipal(identity);

      // cookie contains JWT, JWT contains claims.
      // to update cookie, sign out and back in
      HttpContext.SignOutAsync();
      HttpContext.SignInAsync(newPrincipal);
      return RedirectToAction("Settings", "ServiceProvider", new { onboarding = "t" });
    }

    [HttpGet("[controller]/[action]")]
    public IActionResult SignUp()
    {
      //return Challenge()
      return View();
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult Manage()
    {
      var model = new ManageAccountVm()
      { 
        UserRole = _httpContext.GetUserRole()
      };
      return View(model);
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult PasswordReset()
    {
      _telemetry.TrackEvent("PasswordReset");
      string url = GetIefChallengeUrl("B2C_1A_appts_password_reset", "PasswordResetSuccessful");
      return Redirect(url);
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult PasswordResetSuccessful()
    {
      _telemetry.TrackEvent("PasswordResetSuccessful");
      return RedirectToAction("Index", "Dashboard");
    }
  }
}
