using Appts.Models.Document;
using Appts.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public interface IClientRepository
  {
    Task<CreateClientResponse> CreateIfNotExistsAsync(CreateClientRequest request);
    Task<ClientDoc> GetClientAsync(string clientUserId);
    Task<List<string>> GetClientIdListAsync(string spId);
    Task<List<ClientDoc>> GetClientsAsync(string spId, List<string> clientIdList);
    Task UpdateTzIdAsync(UpdateTimeZoneRequest request);
    Task<PatchClientSettingsResponse> PatchClientSettingsAsync(PatchClientSettingsRequest request);
  }
}