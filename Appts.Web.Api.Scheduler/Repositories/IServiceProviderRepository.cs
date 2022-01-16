using Appts.Models.Document;
using Appts.Models.Rest;
using Appts.Models.Rest.ServiceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public interface IServiceProviderRepository
  {
    Task UpdateTzIdAsync(UpdateTimeZoneRequest request);
    Task<List<string>> GetWhitelistEntriesAsync(string serviceProviderId);
    Task PatchWhitelistAsync(PatchWhitelistRequest request);
    Task<bool> IsEmailWhitelisted(string serviceProviderId, string email);
    Task<PatchServiceProviderSettingsResponse> PatchSettingsAsync(PatchServiceProviderSettingsRequest request);
    Task<GetServiceProviderResponse> GetAsync(string serviceProviderId);
    Task<GetSpProfileResponse> GetProfileAsync(GetSpProfileRequest request);
    Task<string> GetTimeZoneIdAsync(string userId);
    Task<string> GetServicProviderUserIdAsync(string serviceProviderVanityUrl);
    Task Create(ServiceProviderDocument sp);
    //Task<bool> CreateSpXClientIfNotExistsAsync(string spid, string clientId);
    Task CreateClientXSpRelationsIfNotExists(string spid, string clientId);
  }
}