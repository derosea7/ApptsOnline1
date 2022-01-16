using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document;
using Appts.Dal.Cosmos;
using Appts.Models.Rest;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public class ClientRepository : IClientRepository
  {
    private readonly IDb _db;
    public ClientRepository(IDb db)
    {
      _db = db;
    }
    public async Task UpdateTzIdAsync(UpdateTimeZoneRequest request)
    {
      var client = await GetClientAsync(request.ClientUserId);
      client.TimeZoneId = request.TimeZoneId;
      await _db.ReplaceNoReturnAsync(client);
    }
    public async Task<ClientDoc> GetClientAsync(string clientUserId)
    {
      string sql = $@"
      select *
      from c
      where
        c.userId = @userId
        and c.entityType = 'Client'
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", clientUserId)
      };
      return await _db.GetSingleAsync<ClientDoc>(sql, parameters);
    }
    public async Task<CreateClientResponse> CreateIfNotExistsAsync(CreateClientRequest request)
    {
      var existingDoc = GetClientDoc(request.ObjectId).GetAwaiter().GetResult();
      if (existingDoc == null)
      { 
        var client = new ClientDoc()
        {
          UserId = request.ObjectId,
          Id = Guid.NewGuid().ToString(),
          DisplayName = request.DisplayName,
          Email = request.Email,
          FirstName = request.GivenName,
          LastName = request.Surname
        };
        await _db.CreateNoReturnAsync(client);      
      }
      return new CreateClientResponse() { Ext_orgId = "Client" };
    }
    public async Task<PatchClientSettingsResponse> PatchClientSettingsAsync(PatchClientSettingsRequest request)
    {
      var response = new PatchClientSettingsResponse()
      {
        PatchSuccessful = false
      };
      try
      {
        ClientDoc clientToPatch = await GetClientAsync(request.UpdatedClient.UserId);
        clientToPatch.MobilePhone = request.UpdatedClient.MobilePhone;
        clientToPatch.Address = request.UpdatedClient.Address;
        clientToPatch.Address2 = request.UpdatedClient.Address2;
        clientToPatch.City = request.UpdatedClient.City;
        clientToPatch.ZipCode = request.UpdatedClient.ZipCode;
        clientToPatch.StateCode = request.UpdatedClient.StateCode;
        clientToPatch.TimeZoneId = request.UpdatedClient.TimeZoneId;
        await _db.ReplaceNoReturnAsync(clientToPatch);
      }
      catch (Exception ex)
      {
        
      }
      response.PatchSuccessful = true;
      return response;
    }
    public async Task<ClientDoc> GetClientDoc(string clientUserId)
    { 
      string sql = $@"
      SELECT  * FROM c
      where c.entityType = 'Client'
      and c.userId = @userId
      ";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", clientUserId)
      };
      ClientDoc client = await _db.GetSingleAsync<ClientDoc>(sql, paramaters);
      return client;
    }
    public async Task<List<string>> GetClientIdListAsync(string spId)
    {
      string sql = $@"
      select value  c.clientId  from c 
      where c.entityType = 'SpXClient'
      and c.userId = @userId
      ";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", spId)
      };
      List<string> clients = await _db.GetMultipeAsync<string>(sql, paramaters);

      return clients;
    }
    public async Task<List<ClientDoc>> GetClientsAsync(string spId, List<string> clientIdList)
    {
      var clients = new List<ClientDoc>();
      if (clientIdList.Count > 0)
      {
        List<string> finalList = new List<string>();
        foreach (string cli in clientIdList)
        {
          finalList.Add($"'{cli}'");
        }
        string thelist = String.Join(',', finalList);
        string sql = $@"
      SELECT  c.displayName, c.timeZoneId, c.email, c.fname, c.lname, c.userId, c.mobilePhone FROM c
      where c.userId in (
      {thelist}
      )
      and c.entityType = 'Client'
      ";
        var paramaters = new List<KeyValuePair<string, string>>()
        {
          new KeyValuePair<string, string>("@userId", spId)
        };
        clients = await _db.GetMultipeAsync<ClientDoc>(sql, paramaters);
      }
      return clients;
    }
  }
  public class ClientList
  {
    public List<string> ClientIds { get; set; }
  }
}
