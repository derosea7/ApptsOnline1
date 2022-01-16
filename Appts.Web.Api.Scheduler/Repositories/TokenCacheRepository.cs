using Appts.Dal.Cosmos;
using Appts.Models.Document;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public class TokenCacheRepository : ITokenCacheRepository
  {
    private readonly IDb _db;
    public TokenCacheRepository(IDb db)
    {
      _db = db;
    }
    public async Task AddAsync(TokenCache tokens)
    {
      await _db.CreateNoReturnAsync(tokens);
    }
    public async Task<TokenCache> GetAsync(string userId, string scope)
    {
      string sql = $@"
      select *
      from c
      where
        c.entityType = 'TokenCache'
        and c.userId = @userId
        and array_contains(c.scopes, @scope)
      ";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId),
        new KeyValuePair<string, string>("@scope", scope)
      };
      return await _db.GetSingleAsync<TokenCache>(sql, paramaters);
    }
    public async Task UpdateAsync(string userId, string scope, TokenCache tokens)
    {
      TokenCache replacement = GetAsync(userId, scope)
        .GetAwaiter().GetResult();
      replacement.AccessToken = tokens.AccessToken;
      replacement.ExpiresInSeconds = tokens.ExpiresInSeconds;
      await _db.ReplaceNoReturnAsync(replacement);
    }
  }
}