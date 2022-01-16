using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Dal.Cosmos;
using Appts.Models.Rest;
using Appts.Models.Api;
using Appts.Models.Document;
using Appts.Models.Domain;
using Appts.Common.Constants;
using Appts.Models.Rest.ServiceProvider;

namespace Appts.Web.Api.Scheduler.Repositories
{
  public class ServiceProviderRepository : IServiceProviderRepository
  {
    private readonly IDb _db;
    private readonly ILookupRepository _lookupRepository;
    public ServiceProviderRepository(IDb db, ILookupRepository lookupRepository)
    {
      _db = db;
      _lookupRepository = lookupRepository;
    }
    public async Task UpdateTzIdAsync(UpdateTimeZoneRequest request)
    {
      var spDoc = await GetDocumentAsync(request.ServiceProviderId);
      spDoc.TimeZoneId = request.TimeZoneId;
      await _db.ReplaceNoReturnAsync(spDoc);
    }
    public async Task<List<string>> GetWhitelistEntriesAsync(string serviceProviderId)
    {
      string sql = $@"SELECT value(e)
      FROM c
      join e in c.emails
      where c.entityType = 'Whitelist'
      and c.userId = @userId";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", serviceProviderId)
      };
      return await _db.GetMultipeAsync<string>(sql, paramaters);
    }
    public async Task PatchWhitelistAsync(PatchWhitelistRequest request)
    {
      WhitelistDocument whitelistToPatch = await GetWhitelistAsync(request.ServiceProviderId);
      if (whitelistToPatch != null)
      {
        whitelistToPatch.Emails = request.WhitelistEntries;
        await _db.ReplaceNoReturnAsync(whitelistToPatch);
      }
      else
      {
        var newWhitelist = new WhitelistDocument()
        {
          Emails = request.WhitelistEntries,
          UserId = request.ServiceProviderId,
          Id = Guid.NewGuid().ToString()
        };
        await _db.CreateNoReturnAsync(newWhitelist);
      }
    }
    public async Task<WhitelistDocument> GetWhitelistAsync(string serviceProviderId)
    {
      string sql = $@"SELECT *
      FROM c
      where c.entityType = 'Whitelist'
      and c.userId = @userId";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", serviceProviderId)
      };
      return await _db.GetSingleAsync<WhitelistDocument>(sql, paramaters);
    }
    // note, preview package of cosmos allowed
    //SELECT value((count(c.id) > 0) ? true : false)
    // newer packages throw error
    public async Task<bool> IsEmailWhitelisted(string serviceProviderId, string email)
    {
      string sql = $@"
      SELECT value c.id
      FROM c
      where c.entityType = 'Whitelist'
      and c.userId = @userId
      and array_contains(c.emails, @email)
      ";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", serviceProviderId),
        new KeyValuePair<string, string>("@email", email)
      };
      string whitelistId = await _db.GetSingleAsync<string>(sql, paramaters);
      bool result = false;
      if (!string.IsNullOrEmpty(whitelistId))
        result = true;
      return result;
    }
    public async Task<PatchServiceProviderSettingsResponse> PatchSettingsAsync(
      PatchServiceProviderSettingsRequest request)
    {
      var response = new PatchServiceProviderSettingsResponse();
      ServiceProviderDocument spToPatch = await GetDocumentAsync(request.ServiceProviderId);
      if (spToPatch.VanityUrl != request.VanityUrl)
      {
        // check if url is taken
        string vanityUrlLookup = await _lookupRepository.GetAsync(EntityK.VanityUrlLookup, request.VanityUrl);
        if (!string.IsNullOrEmpty(vanityUrlLookup))
        {
          response.IsVanityUrlTaken = true;
          return response;
        }
      }
      string originalVanityUrl = spToPatch.VanityUrl;
      // maps updates
      spToPatch.DisplayName = request.DisplayName;
      spToPatch.VanityUrl = request.VanityUrl;
      spToPatch.RequireMyConfirmation = request.RequireMyConfirmation;
      spToPatch.SchedulingPrivacyLevel = (SchedulingPrivacyLevel)request.SchedulingPrivacyLevel;
      spToPatch.TimeZoneId = request.TimeZoneId;
      spToPatch.MobilePhone = request.MobilePhone;
      //businessToPatch.AllowRecurringAppointments = request.Business.AllowRecurringAppointments;
      //businessToPatch.OnlyAllowConflictFreeRecurrences = request.Business.OnlyAllowConflictFreeRecurrences;
      //TODO: update lookup with next vanity url
      await _lookupRepository.PatchAsync(EntityK.VanityUrlLookup, originalVanityUrl, null, request.VanityUrl);
      await _db.ReplaceAsync<ServiceProviderDocument>(spToPatch);
      return response;
    }
    public async Task<ServiceProviderDocument> GetDocumentAsync(string serviceProviderId)
    {
      string sql = $@"
      select *
      from c
      where
        c.userId = @userId
        and c.entityType = 'ServiceProvider'
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", serviceProviderId)
      };
      return await _db.GetSingleAsync<ServiceProviderDocument>(sql, parameters);
    }
    public async Task<GetServiceProviderResponse> GetAsync(string serviceProviderId)
    {
      var spDocument = await GetDocumentAsync(serviceProviderId);
      if (spDocument == null) return new GetServiceProviderResponse();
      return new GetServiceProviderResponse()
      {
        DisplayName = spDocument.DisplayName,
        RequireMyConfirmation = spDocument.RequireMyConfirmation,
        SchedulingPrivacyLevel = (int)spDocument.SchedulingPrivacyLevel,
        TimeZoneId = spDocument.TimeZoneId,
        VanityUrl = spDocument.VanityUrl,
        Email = spDocument.Email,
        MobilePhone = spDocument.MobilePhone
      };
    }
    public async Task<GetSpProfileResponse> GetProfileAsync(GetSpProfileRequest request)
    {
      var spDocument = await GetDocumentAsync(request.Spid);
      return new GetSpProfileResponse() { Sp = spDocument };
    }
    public async Task<string> GetTimeZoneIdAsync(string userId)
    {
      string sql = $@"
      SELECT value (c.timeZoneId ?? '') 
      FROM c
      where
        c.entityType = 'ServiceProvider'
        and c.userId = '{userId}'
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId)
      };
      return await _db.GetSingleAsync<string>(sql, parameters);
    }
    public async Task<string> GetServicProviderUserIdAsync(string serviceProviderVanityUrl)
    {
      //string sql = $@"
      //select value c.data.val
      //from c
      //where
      //  c.userId = 'Lookups'
      //  and c.entityType = 'VanityUrlToServiceProviderUserId'
      //  and lower(c.data.key) = '{serviceProviderVanityUrl.ToLower()}' 
      //";
      //return await _db.GetSingleAsync<string>(sql);
      return await _lookupRepository.GetAsync(EntityK.VanityUrlLookup, serviceProviderVanityUrl);
    }
    //public async Task Create(OnboardServiceProviderRequest request)
    public async Task Create(ServiceProviderDocument sp)
    {
      await _db.CreateNoReturnAsync(sp);
    }
    public async Task CreateClientXSpRelationsIfNotExists(string spid, string clientId)
    {
      bool exists = CreateSpXClientIfNotExistsAsync(spid, clientId).GetAwaiter().GetResult();
      if (!exists)
        await CreateClientXSpIfNotExistsAsync(spid, clientId);
    }
    private async Task<bool> CreateSpXClientIfNotExistsAsync(string spid, string clientId)
    {
      string sql = $@"
      select * from c 
      where 
        c.userId = @spid
        and c.entityType = 'SpXClient' 
        and c.clientId = @clientId
      ";
      var args = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@spid", spid),
        new KeyValuePair<string, string>("@clientId", clientId)
      };
      var spxclient = await _db.GetSingleAsync<SpXClientDocument>(sql, args);
      bool exists = true;
      if (spxclient == null)//nothing found, therefor need to create
      {
        exists = false;
        var newx = new SpXClientDocument()
        {
          UserId = spid,
          ClientId = clientId,
          Id = Guid.NewGuid().ToString()
        };
        await _db.CreateNoReturnAsync(newx);
      }
      return exists;
    }
    private async Task CreateClientXSpIfNotExistsAsync(string spid, string clientId)
    {
      string sql = $@"
      select * from c 
      where 
        c.userId = @clientId
        and c.entityType = 'ClientXSp' 
        and c.serviceProviderId = @spid
      ";
      var args = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@spid", spid),
        new KeyValuePair<string, string>("@clientId", clientId)
      };
      var clientexsp = await _db.GetSingleAsync<ClientXSpDoc>(sql, args);
      if (clientexsp == null)//nothing found, therefor need to create
      {
        var newx = new ClientXSpDoc()
        {
          UserId = clientId,
          ServiceProviderId = spid,
          Id = Guid.NewGuid().ToString()
        };
        await _db.CreateNoReturnAsync(newx);
      }
    }
  }
}
