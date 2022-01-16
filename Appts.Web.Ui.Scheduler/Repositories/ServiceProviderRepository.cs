using Appts.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Repositories
{
  public class ServiceProviderRepository : IServiceProviderRepository
  {
    private readonly IApiClient _apiClient;
    public ServiceProviderRepository(IApiClient apiClient)
    {
      _apiClient = apiClient;
    }
    public async Task UpdateTzIdAsync(UpdateTimeZoneRequest request)
    {
      await _apiClient.PatchNoReturnAsync<UpdateTimeZoneRequest>(
        request, $"/api/ServiceProvider/UpdateTzId");
    }
    public async Task<List<string>> GetWhitelistEntriesAsync(string serviceProviderId)
    {
      return await _apiClient.GetAsync<List<string>>(
        $"/api/ServiceProvider/GetWhitelistEntries/{serviceProviderId}");
    }

    public async Task<GetSchedulingPrivacyLevelResponse> GetSchedulingPrivacyLevelAsync(string serviceProviderVanityUrl)
    {
      return await _apiClient.GetAsync<GetSchedulingPrivacyLevelResponse>(
        $"/api/ServiceProvider/GetSchedulingPrivacyLevel/{serviceProviderVanityUrl}");
    }

    public async Task<bool> IsEmailWhitelisted(string serviceProviderId, string email)
    {
      return await _apiClient.GetAsync<bool>(
        $"/api/ServiceProvider/IsEmailWhitelisted/{serviceProviderId}?email={email}");
    }
  }
}
