using System.Threading.Tasks;
namespace Appts.Web
{
  public interface IApiClient
  {
    // oauth
    Task SetToken();

    Task<string> GetStringAsync(string endpoint);
    Task<TOut> GetAsync<TOut>(string endpoint);
    Task PatchNoReturnAsync<T>(T patch, string endpoint);
    Task<TOut> PatchAsync<T, TOut>(T patch, string endpoint);
    Task PostNoReturnAsync<T>(T objectToPost, string endpoint);
    Task<T> PostAsync<T>(T objectToPost, string endpoint);
    Task<TOut> PostAsync<TIn, TOut>(TIn objectToPost, string endpoint);
    void ChangeBaseAddress(string newBaseUrl);
    void RemoveAuthHeader();
  }
}
