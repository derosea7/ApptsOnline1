using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Appts.Models.Document;
using Appts.Models.Docs;
namespace Appts.Dal.Cosmos
{
  /// <summary>
  /// Object to interact with a db and container specified at intialization time.
  /// </summary>
  public class Db : IDb
  {
    #region "CosmosDb"
    private CosmosClient _cosmosClient;
    private Database _database;
    private Container _container;
    private string _databaseId;
    private string _containerId;
    private string _endpoint;
    private string _primaryKey;
    //private const string _partitionKey = "/pk";
    private string _partitionKey;
    public Db(string databaseId, string containerId, string endpoint, string primaryKey, string partitionKey)
    {
      _databaseId = databaseId;
      _containerId = containerId;
      _endpoint = endpoint;
      _primaryKey = primaryKey;
      _partitionKey = partitionKey;
      InitializeAsync().GetAwaiter().GetResult();
    }
    /// <summary>
    /// Create connection to specified container.
    /// </summary>
    /// <returns>Nothing if successful, exception otherwise.</returns>
    public async Task InitializeAsync()
    {
      // Create a new instance of the Cosmos Client
      this._cosmosClient = new CosmosClient(_endpoint, _primaryKey);
      await this.CreateDatabaseAsync();
      await this.CreateContainerAsync();
    }
    private async Task CreateDatabaseAsync()
    {
      this._database = await this._cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseId);
    }
    private async Task CreateContainerAsync()
    {
      this._container = await this._database.CreateContainerIfNotExistsAsync(_containerId, _partitionKey);
    }
    #endregion
    //public async Task<TOut> GetSingleAsync2<TOut>(string sqlQuery, List<KeyValuePair<string, string>> sqlParameters = null)
    //{
    //  //var query = new CosmosSqlQueryDefinition("");

    //  //var q = new QueryDefinition(sqlQuery);
    //  //if (sqlParameters != null)
    //  //{
    //  //  foreach (KeyValuePair<string, string> paramater in sqlParameters)
    //  //  {
    //  //    q.WithParameter(paramater.Key, paramater.Value);
    //  //  }
    //  //}
    //  //var iterator = this._container.GetItemQueryIterator<TOut>(q);
    //  //FeedResponse<TOut> cursor = await iterator.ReadNextAsync();
    //  //return cursor.FirstOrDefault<TOut>();
    //}
    /// <summary>
    /// Get a single document of type TOut, given a sql query to find it.
    /// </summary>
    /// <typeparam name="TOut">Type of document the query will return.</typeparam>
    /// <param name="sqlQuery">Query db will execute.</param>
    /// <param name="sqlParameters">Query parameters to parameterize using SDK.</param>
    /// <returns>Document of type TOut if found, null if not.</returns>
    public async Task<TOut> GetSingleAsync<TOut>(string sqlQuery, List<KeyValuePair<string, string>> sqlParameters = null)
    {
      var q = new QueryDefinition(sqlQuery);
      if (sqlParameters != null)
      {
        foreach (KeyValuePair<string, string> paramater in sqlParameters)
        {
          q.WithParameter(paramater.Key, paramater.Value);
        }
      }
      var iterator = this._container.GetItemQueryIterator<TOut>(q);
      FeedResponse<TOut> cursor = await iterator.ReadNextAsync();
      return cursor.FirstOrDefault<TOut>();
    }
    /// <summary>
    /// Get a list of documents of type TOut, given a sql query to find them.
    /// </summary>
    /// <typeparam name="TOut">Type of document in list the query will return.</typeparam>
    /// <param name="sqlQuery">Query db will execute.</param>
    /// <param name="sqlParameters">Query parameters to parameterize using SDK.</param>
    /// <returns>List of Documents of type TOut if found, null if not.</returns>
    public async Task<List<TOut>> GetMultipeAsync<TOut>(string sqlQuery, List<KeyValuePair<string, string>> sqlParameters = null)
    {
      var q = new QueryDefinition(sqlQuery);
      if (sqlParameters != null)
      {
        foreach (KeyValuePair<string, string> paramater in sqlParameters)
        {
          q.WithParameter(paramater.Key, paramater.Value);
        }
      }
      var iterator = this._container.GetItemQueryIterator<TOut>(q);
      var things = new List<TOut>();
      while (iterator.HasMoreResults)
      {
        FeedResponse<TOut> currentResultSet = await iterator.ReadNextAsync();
        foreach (TOut thing in currentResultSet)
        {
          things.Add(thing);
        }
      }
      return things;
    }
    public async Task ReplaceNoReturnAsync(Document replacement)
    {
      await this._container.ReplaceItemAsync<Document>(
        replacement, replacement.Id, new PartitionKey(replacement.UserId));
    }
    public async Task<T> ReplaceAsync<T>(IDocument replacement)
    {
      var result = await this._container.ReplaceItemAsync<T>(
        (T)replacement, replacement.Id, new PartitionKey(replacement.UserId));
      return (T)result.Resource;
    }
    public async Task CreateNoReturnAsync(Document document)
    {
      ItemResponse<Document> response =
        await this._container.CreateItemAsync<Document>(document, new PartitionKey(document.UserId));
    }
    public async Task CreateNoReturnAsync2(Doc document)
    {
      ItemResponse<Doc> response =
        await this._container.CreateItemAsync<Doc>(document, new PartitionKey(document.PartitionKey));
    }
    public async Task DeleteNoReturnAsync<T>(string documentId, string userId)
    {
      await this._container.DeleteItemAsync<T>(documentId, new PartitionKey(userId));
    }
    //note, this returns an error parsing the response, but actually deletes data
    // must pass params as dynamic array, hence new[] {}
    //must have partition id
    public async Task<SpResult_BulkDataDelete> ExecuteStoredProcAsync(string query, string userId)
    {
      SpResult_BulkDataDelete result = null;
      result = await this._container.Scripts.ExecuteStoredProcedureAsync<SpResult_BulkDataDelete>(
        "bulkDeleteStoredProcedure",
        new PartitionKey(userId),
        new[] { query },
        new Microsoft.Azure.Cosmos.Scripts.StoredProcedureRequestOptions() { EnableScriptLogging = true });
      //string log = result.ScriptLog;
      
      return result;
    }
  }
}