using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Repositories
{
  public interface ITokenCacheRepository
  {
    Task AddAsync(TokenCache tokens);
    Task<TokenCache> GetAsync(string userId, string scope);
    Task UpdateAsync(string userId, string scope, TokenCache tokens);
  }
}
