using Appts.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Repositories
{
  public interface IServiceProviderRepository
  {
    Task<List<string>> GetWhitelistEntriesAsync(string serviceProviderId);
    Task<GetSchedulingPrivacyLevelResponse> GetSchedulingPrivacyLevelAsync(string serviceProviderVanityUrl);
    Task<bool> IsEmailWhitelisted(string serviceProviderId, string email);
    Task UpdateTzIdAsync(UpdateTimeZoneRequest request);
  }
}
