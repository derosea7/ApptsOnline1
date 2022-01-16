using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document;
using Appts.Web.Api.Scheduler.Repositories;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Appts.Web.Api.Scheduler.Controllers
{
  //[Authorize]
  [Route("api/[controller]")]
  public class TokenCacheController : Controller
  {
    private readonly ITokenCacheRepository _tokenCacheRepository;

    public TokenCacheController(ITokenCacheRepository tokenCacheRepository)
    {
      _tokenCacheRepository = tokenCacheRepository;
    }

    // GET api/<controller>/5
    [HttpGet("{userId}")]
    public TokenCache Get(string userId, string scope)
    {
      //return _Db.GetTokenCache(userId, scope).GetAwaiter().GetResult();
      return _tokenCacheRepository.GetAsync(userId, scope).GetAwaiter().GetResult();
    }

    // POST api/<controller>
    [HttpPost]
    public void Post([FromBody] TokenCache tokens)
    {
      //_Db.CreateTokenCache(tokens).GetAwaiter().GetResult();
      _tokenCacheRepository.AddAsync(tokens).GetAwaiter().GetResult();
    }

    [HttpPatch("{userId}")]
    public void Put(string userId, [FromBody] TokenCache tokens)
    {
      string scope = tokens.Scopes[0];
      _tokenCacheRepository.UpdateAsync(userId, scope, tokens).GetAwaiter().GetResult();
    }

    // DELETE api/<controller>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
