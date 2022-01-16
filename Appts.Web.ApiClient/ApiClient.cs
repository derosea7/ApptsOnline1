using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Dal.RedisCache;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Appts.Web
{
  public class ApiClient : IApiClient
  {
    private readonly string _resourceId;
    private readonly string _authority;
    private readonly string _appId;
    private readonly string _appSecret;
    private readonly HttpClient _client;
    private readonly IRedisConnectionFactory _cache;
    private IDatabase _db;
    private string _logToken;
    private bool _tokenSet = false;

    public ApiClient(
      HttpClient client, IConfiguration config,
      IRedisConnectionFactory cache)
    {
      _resourceId = config["Api:ResourceId"];
      _authority = $"{config["AzureAdB2C:Instance"]}{config["AzureAdB2C:TenantId"]}";
      _appId = config["AzureAdB2C:ClientId"];
      _appSecret = config["AzureAdB2C:ClientSecret"];
      this._client = client;
      this._client.BaseAddress = new Uri(config["Api:BaseUrl"]);
      _cache = cache;
      _logToken = config["FeatureToggle:LogAccessToken"];
    }
    public void ChangeBaseAddress(string newBaseUrl)
    {
      this._client.BaseAddress = new Uri(newBaseUrl);
    }
    public void RemoveAuthHeader()
    {
      _client.DefaultRequestHeaders.Authorization = null;
    }
    private void InitializeCache()
    {
      try
      {
        _db = _cache.Connection().GetDatabase();
      }
      catch (RedisException ex)
      {
      }
    }

    public static string GetB2CAccessTokenKey()
    {
      return $"b2c:access_token";
    }

    private string TryGetCachedAccessToken()
    {
      string cachedToken = "";
      string tokenKey = GetB2CAccessTokenKey();
      if (_db.Multiplexer.IsConnected == true)
      {
        try
        {
          cachedToken = _db.StringGet(tokenKey);
        }
        catch (RedisException ex)
        {
        }
      }

      return cachedToken;
    }
    public async Task SetToken()
    {
      if (_tokenSet)
        return;

      //InitializeCache();

      string cachedToken = "";
      //cachedToken = TryGetCachedAccessToken();
      string token = cachedToken;
      //cachedToken = "";
      if (string.IsNullOrEmpty(cachedToken))
      {
        var authContext = new AuthenticationContext(_authority);
        var credential = new ClientCredential(_appId, _appSecret);

        var authResult = await authContext.AcquireTokenAsync(_resourceId, credential);
        token = authResult.AccessToken;

        //TryCacheAccessToken(token, authResult.ExpiresOn);
      }

      if (!string.IsNullOrEmpty(token))
      {
        _client.DefaultRequestHeaders.Authorization =
          new AuthenticationHeaderValue("Bearer", token);
        _tokenSet = true;

        try
        {
          if (_logToken == "true")
          {
            var logPath = @"C:\Users\derosea7\Desktop\tokens.txt";
            System.IO.File.AppendAllText(logPath, $"\r\n \r\n \r\n{token}");
          }
        }
        catch (Exception ex)
        {
          // do nothing
        }
      }
    }
    private void TryCacheAccessToken(string accessToken, DateTimeOffset tokenExpiration)
    {
      // note that _userId will be = "" when anonymous user on site
      if (_db.Multiplexer.IsConnected == true)
      {
        TimeSpan expiration = tokenExpiration.Subtract(DateTime.UtcNow);
        string tokenKey = GetB2CAccessTokenKey();
        try
        {
          _db.StringSet(tokenKey, accessToken, expiration);
        }
        catch (RedisException ex) { }
      }
    }

    #region "Helpers"

    public async Task<string> GetStringAsync(string endpoint)
    {
      await SetToken();
      var response = await _client.GetAsync(endpoint);
      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"{response.StatusCode}");
      }
      var content = await response.Content.ReadAsStringAsync();
      return content;
    }

    public async Task<TOut> GetAsync<TOut>(string endpoint)
    {
      await SetToken();
      var content = await GetStringAsync(endpoint);
      return JsonConvert.DeserializeObject<TOut>(content);
    }

    public async Task PatchNoReturnAsync<T>(T patch, string endpoint)
    {
      await SetToken();
      string json = JsonConvert.SerializeObject(patch);
      var response = await _client.PatchAsync(endpoint, AsJsonContent(json));
      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"{response.StatusCode}");
      }
    }

    public async Task<TOut> PatchAsync<T, TOut>(T patch, string endpoint)
    {
      await SetToken();
      string json = JsonConvert.SerializeObject(patch);
      var response = await _client.PatchAsync(endpoint, AsJsonContent(json));
      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"{response.StatusCode}");
      }
      string content = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<TOut>(content);
    }

    /// <summary>
    /// Send post and throw exception if fails.
    /// </summary>
    /// <typeparam name="TOut">Type of object to serialize and post</typeparam>
    /// <param name="objectToPost">Object to serializ and post</param>
    /// <param name="endpoint">Uri to send POST</param>
    /// <returns>Nothing--throws exception if operation fails.</returns>
    public async Task PostNoReturnAsync<T>(T objectToPost, string endpoint)
    {
      await SetToken();
      HttpResponseMessage response;
      try
      {
        string json = JsonConvert.SerializeObject(objectToPost);
        response = await _client.PostAsync(endpoint, AsJsonContent(json));

      }
      catch (Exception ex)
      {

        throw;
      }
      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"{response.StatusCode}");
      }
    }

    public async Task<T> PostAsync<T>(T objectToPost, string endpoint)
    {
      await SetToken();
      string businessJson = JsonConvert.SerializeObject(objectToPost);
      var response = await _client.PostAsync(endpoint, AsJsonContent(businessJson));
      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"{response.StatusCode}");
      }
      var content = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<T>(content);
    }

    public async Task<TOut> PostAsync<TIn, TOut>(TIn objectToPost, string endpoint)
    {
      await SetToken();
      string businessJson = JsonConvert.SerializeObject(objectToPost);
      var response = await _client.PostAsync(endpoint, AsJsonContent(businessJson));
      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"{response.StatusCode}");
      }
      var content = await response.Content.ReadAsStringAsync();

      return JsonConvert.DeserializeObject<TOut>(content);
    }

    StringContent AsJsonContent(string json)
    {
      return new StringContent(json, Encoding.UTF8, "application/json");
    }

    #endregion

  }
}
