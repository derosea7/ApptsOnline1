using System.Collections.Generic;
using System.Threading.Tasks;
using Appts.Models.Document;
using Appts.Models.Docs;
namespace Appts.Dal.Cosmos
{
  public interface IDb
  {
    Task InitializeAsync();
    Task ReplaceNoReturnAsync(Document replacement);
    Task<T> ReplaceAsync<T>(IDocument replacement);
    Task<List<TOut>> GetMultipeAsync<TOut>(string sqlQuery, List<KeyValuePair<string, string>> sqlParameters = null);
    Task<TOut> GetSingleAsync<TOut>(string sqlQuery, List<KeyValuePair<string, string>> sqlParameters = null);
    Task CreateNoReturnAsync(Document document);
    Task CreateNoReturnAsync2(Doc document);
    Task DeleteNoReturnAsync<T>(string documentId, string userId);
    Task<SpResult_BulkDataDelete> ExecuteStoredProcAsync(string query, string userId);
  }
}
