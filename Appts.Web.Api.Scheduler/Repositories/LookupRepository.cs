using Appts.Dal.Cosmos;
using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public class LookupRepository : ILookupRepository
  {
    private readonly IDb _db;
    public LookupRepository(IDb db)
    {
      _db = db;
      //db.InitializeAsync().GetAwaiter().GetResult();
    }
    /// <summary>
    /// Returns entire lookup document, given a key and parition id.
    /// </summary>
    /// <typeparam name="T">Type of document will be a lookup document.</typeparam>
    /// <param name="lookupPartition">user id which is container parition id, houses lookup docs</param>
    /// <param name="key">key to find lookup document</param>
    /// <returns>entire lookup document containing key and value</returns>
    public async Task<T> GetDocumentAsync<T>(string lookupPartition, string key)
    {
      string sql = $@"
      SELECT *
      FROM c
      where 
        c.userId = @lookupPartition
        and c.k = @key
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@lookupPartition", lookupPartition),
        new KeyValuePair<string, string>("@key", key)
      };
      return await _db.GetSingleAsync<T>(sql, parameters);
    }
    /// <summary>
    /// Get a single value, given a key and partion.
    /// </summary>
    /// <param name="lookupPartition">UserId, which is partition key of container, of look up documents.</param>
    /// <param name="key">key to find value in lookup partition</param>
    /// <returns>Value given a lookup key.</returns>
    public async Task<string> GetAsync(string lookupPartition, string key)
    {
      string sql = $@"
      SELECT value c.v 
      FROM c
      where 
        c.userId = @lookupPartition
        and lower(c.k) = @key
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@lookupPartition", lookupPartition),
        new KeyValuePair<string, string>("@key", key.ToLower())
      };
      return await _db.GetSingleAsync<string>(sql, parameters);
    }
    /// <summary>
    /// Create new lookup document.
    /// </summary>
    /// <param name="lookupPartition">user id or partiion id of lookup documents</param>
    /// <param name="key">key to save</param>
    /// <param name="value">value to save</param>
    /// <returns>Nothing if successful, exception otherwise.</returns>
    public async Task PostAsync(string lookupPartition, string key, string value)
    {
      var lookup = new LookupDocument()
      {
        Id = Guid.NewGuid().ToString(),
        UserId = lookupPartition,
        Key = key,
        Value = value
      };
      await _db.CreateNoReturnAsync(lookup);
    }
    public async Task PatchAsync(string lookupPartition, string key, string newValue = null, string newKey = null)
    {
      if (string.IsNullOrEmpty(newValue) && string.IsNullOrEmpty(newKey))
        throw new ArgumentNullException("Must provide either new key or value");
      var lookupDocument = await GetDocumentAsync<LookupDocument>(lookupPartition, key);
      if (!string.IsNullOrEmpty(newValue))
        lookupDocument.Value = newValue;
      if (!string.IsNullOrEmpty(newKey))
        lookupDocument.Key = newKey;
      await _db.ReplaceNoReturnAsync(lookupDocument);
    }
  }
}
