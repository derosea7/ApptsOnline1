using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document;
using Appts.Models.Rest;
using Appts.Models.Rest.ServiceProvider;
using Appts.Models.Domain;
using Appts.Web.Api.Scheduler.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace Appts.Web.Api.Scheduler.Controllers
{
  //[Route("api/[controller]/[action]/{id}")]
  public class ServiceProviderController : Controller
  {
    private readonly IServiceProviderRepository _serviceProviderRepository;

    public ServiceProviderController(IServiceProviderRepository serviceProviderRepository)
    {
      _serviceProviderRepository = serviceProviderRepository;
    }

    [Route("api/[controller]/[action]/{serviceProviderId}")]
    public List<string> GetWhitelistEntries(string serviceProviderId)
    {
      return _serviceProviderRepository.GetWhitelistEntriesAsync(serviceProviderId)
        .GetAwaiter().GetResult();
    }

    [HttpPatch]
    [Route("api/[controller]/[action]")]
    public void PatchWhitelist([FromBody] PatchWhitelistRequest request)
    {
      _serviceProviderRepository.PatchWhitelistAsync(request)
        .GetAwaiter().GetResult();
    }

    [Route("api/[controller]/[action]/{serviceProviderId}")]
    public bool IsEmailWhitelisted(string serviceProviderId, string email)
    {
      return _serviceProviderRepository.IsEmailWhitelisted(serviceProviderId, email)
        .GetAwaiter().GetResult();
    }

    [HttpPatch("api/[controller]/[action]")]
    public void UpdateTzId([FromBody]UpdateTimeZoneRequest request)
    {
      _serviceProviderRepository.UpdateTzIdAsync(request).GetAwaiter().GetResult();
    }

    [HttpPatch]
    [Route("api/[controller]/[action]")]
    public PatchServiceProviderSettingsResponse PatchSettings([FromBody]PatchServiceProviderSettingsRequest request)
    {
      return _serviceProviderRepository.PatchSettingsAsync(request).GetAwaiter().GetResult();
    }

    //[Route("api/[controller]/[action]/{serviceProviderId}")]
    [Route("api/[controller]/{serviceProviderId}")]
    public GetServiceProviderResponse Get(string serviceProviderId)
    {
      return _serviceProviderRepository.GetAsync(serviceProviderId).GetAwaiter().GetResult();
    }

    //[HttpPost("api/[controller]/[action]")]
    //public void Create(OnboardServiceProviderRequest request)
    //{
    //  _serviceProviderRepository.Create(request).GetAwaiter().GetResult();
    //}

    [Route("api/[controller]/[action]/{serviceProviderVanityUrl}")]
    public GetSchedulingPrivacyLevelResponse GetSchedulingPrivacyLevel(string serviceProviderVanityUrl)
    {
      var model = new GetSchedulingPrivacyLevelResponse();
      string serviceProviderId = _serviceProviderRepository
        .GetServicProviderUserIdAsync(serviceProviderVanityUrl)
        .GetAwaiter().GetResult();

      if (serviceProviderId == null)
      {
        model.FoundServiceProvider = false;
        return model;
      }

      var response = _serviceProviderRepository.GetAsync(serviceProviderId)
        .GetAwaiter().GetResult();

      model.ServiceProviderId = serviceProviderId;
      model.FoundServiceProvider = true;
      model.ServiceProviderEmail = response.Email;
      model.SchedulingPrivacyLevel = (SchedulingPrivacyLevel)response.SchedulingPrivacyLevel;

      return model;
    }
    [Route("api/[controller]/[action]/{spid}")]
    public GetSpProfileResponse GetProfile(GetSpProfileRequest request)
    {
      return _serviceProviderRepository.GetProfileAsync(request).GetAwaiter().GetResult();
    }
  }
}
