using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Repositories
{
  public class TokenCacheRepository : ITokenCacheRepository
  {
    private readonly IApiClient _apiClient;
    public TokenCacheRepository(IApiClient apiClient)
    {
      _apiClient = apiClient;
    }

    public async Task AddAsync(TokenCache tokens)
    {
      await _apiClient.PostNoReturnAsync<TokenCache>(tokens,
        "/api/tokencache");
    }

    public async Task<TokenCache> GetAsync(string userId, string scope)
    {
      return await _apiClient.GetAsync<TokenCache>(
        $"/api/tokencache/{userId}?scope={scope}");
    }

    public async Task UpdateAsync(string userId, string scope, TokenCache tokens)
    {
      // put scope in body of request for convience?
      tokens.Scopes = new string[] { scope };

      await _apiClient.PatchNoReturnAsync<TokenCache>(tokens,
        $"/api/tokencache/{userId}");
      //$"/api/tokencache/{userId}?scope={scope}");
    }
  }
}
