using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Appts.Models.View;
using Appts.Common.Constants;
using Appts.Web.Ui.Scheduler.Repositories;
using Appts.Models.Rest;
using Microsoft.AspNetCore.Authorization;
using Appts.Models.Rest.ServiceProvider;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  /// <summary>
  /// Controls interactions with service provider.
  /// </summary>
  [Authorize]
  public class ServiceProviderController : Controller
  {
    private readonly IApiClient _apiClient;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly ILogger<ServiceProviderController> _logger;
    private readonly IBlobAvatarRepository _blobAvatarRepository;
    private TelemetryClient _telemetry;
    public ServiceProviderController(
      IApiClient apiClient, IServiceProviderRepository serviceProviderRepository,
      ILogger<ServiceProviderController> logger,
      IBlobAvatarRepository blobAvatarRepository,
      TelemetryClient telemetry)
    {
      _apiClient = apiClient;
      _serviceProviderRepository = serviceProviderRepository;
      _logger = logger;
      _blobAvatarRepository = blobAvatarRepository;
      _telemetry = telemetry;
    }
    public IActionResult Dashboard()
    {
      return View();
    }
    public IActionResult Index()
    {
      return View();
    }
    [HttpPost]
    public bool UpdateTzId([FromBody] UpdateTimeZoneViewModel model)
    {
      _telemetry.TrackEvent("TzUpdated");
      bool wasUpdated = false;
      var request = new UpdateTimeZoneRequest()
      {
        ServiceProviderId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value,
        TimeZoneId = model.TimeZoneId
      };
      _serviceProviderRepository.UpdateTzIdAsync(request).GetAwaiter().GetResult();
      wasUpdated = true; // if we made it here w/o ex
      return wasUpdated;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="onboarding"></param>
    /// <returns></returns>
    public IActionResult Settings(string onboarding = null)
    {
      _telemetry.TrackPageView("SpSettingsViewed");
      string spId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      //lotta work here to impl default avatar, not going to use going forward
      //if (onboarding == "t")
      //{
      //  _blobAvatarRepository.CreateDefaultAvatar(spId);
      //}
      var model = new UpdateServiceProviderSettingsViewModel();
      var response = _apiClient.GetAsync<GetServiceProviderResponse>(
        $"/api/ServiceProvider/{spId}").GetAwaiter().GetResult();
      model.DisplayName = response.DisplayName;
      model.RequireMyConfirmation = response.RequireMyConfirmation;
      model.SchedulingPrivacyLevel = response.SchedulingPrivacyLevel;
      model.TimeZoneId = response.TimeZoneId;
      model.VanityUrl = response.VanityUrl;
      model.SpId = spId;
      model.MobilePhone = response.MobilePhone;
      if (onboarding == "t")
      {
        model.IsOnboarding = true;
      }
      return View(model);
    }
    [HttpPost]
    public IActionResult Settings(UpdateServiceProviderSettingsViewModel model)
    {
      _telemetry.TrackEvent("SpSettingsUpdated");
      if (ModelState.IsValid)
      {
        var request = new PatchServiceProviderSettingsRequest()
        {
          ServiceProviderId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value,
          DisplayName = model.DisplayName,
          RequireMyConfirmation = model.RequireMyConfirmation,
          SchedulingPrivacyLevel = model.SchedulingPrivacyLevel,
          TimeZoneId = model.TimeZoneId,
          VanityUrl = model.VanityUrl,
          MobilePhone = model.MobilePhone
        };
        PatchServiceProviderSettingsResponse response = _apiClient
          .PatchAsync<PatchServiceProviderSettingsRequest, PatchServiceProviderSettingsResponse>(
            request,
            $"/api/ServiceProvider/PatchSettings").GetAwaiter().GetResult();
        if (response != null && response.IsVanityUrlTaken)
        {
          ModelState.AddModelError("VanityUrl", "The vanity url is not available");
          return View("Settings", model);
        }
        return RedirectToAction("Settings", new { updated = "success" });
      }
      return View(model);
    }
    [HttpGet]
    public List<string> GetWhitelistEntries()
    {
      string serviceProviderId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      return _apiClient.GetAsync<List<string>>(
        $"/api/ServiceProvider/GetWhitelistEntries/{serviceProviderId}")
        .GetAwaiter().GetResult();
    }
    [HttpPost]
    public bool UpdateWhitelist([FromBody] PatchWhitelistViewModel whitelist)
    {
      _telemetry.TrackEvent("WhitelistUpdated");
      var request = new PatchWhitelistRequest()
      {
        ServiceProviderId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value,
        WhitelistEntries = whitelist.WhitelistEntries
      };
      //_businessRepository.UpdateWhitelistAsync(request).GetAwaiter().GetResult();
      _apiClient.PatchNoReturnAsync<PatchWhitelistRequest>(request,
        $"/api/ServiceProvider/PatchWhitelist");
      // true creates success status for jquery
      return true;
    }
    //[HttpGet("[controller]/[action]")]
    //public IActionResult ScheduledWith()
    //{ 
    //}
    [HttpGet("[controller]/[action]/{spid}")]
    public IActionResult Profile(string spid)
    {
      _telemetry.TrackPageView("SpProfileViewed");
      var endpoint = $"/api/ServiceProvider/GetProfile/{spid}";
      var response = _apiClient.GetAsync<GetSpProfileResponse>(endpoint)
        .GetAwaiter().GetResult();
      var vm = new ServiceProviderProfileViewModel();
      if (response != null)
      {
        vm = new ServiceProviderProfileViewModel()
        {
          FoundSp = true,
          SpDisplayName = response.Sp.DisplayName,
          SpEmail = response.Sp.Email,
          SpFName = response.Sp.FirstName,
          SpLName = response.Sp.LastName,
          SpVanityUrl = response.Sp.VanityUrl
        };
      }
      else
      {
        vm = new ServiceProviderProfileViewModel()
        {
          FoundSp = false
        };
      }
      return View(vm);
    }
  }
}