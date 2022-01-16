using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Dal.Cosmos;
using Appts.Models.Rest;
using Appts.Web.Api.Scheduler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Appts.Web.Api.Scheduler.Controllers
{
  [Authorize]
  [Route("api/[controller]")]
  public class LookupController : Controller
  {
    private readonly ILookupRepository _lookupRepository;
    public LookupController(ILookupRepository lookupRepository)
    {
      _lookupRepository = lookupRepository;
    }
    [HttpGet("{lookupPartition}")]
    public string Get(string lookupPartition, string key)
    {
      return _lookupRepository.GetAsync(lookupPartition, key).GetAwaiter().GetResult();
    }
    [HttpPost]
    public void Post([FromBody]AddLookupRequest request)
    {
      _lookupRepository.PostAsync(request.PartitionId, request.Key, request.Value)
        .GetAwaiter().GetResult();
    }
    [HttpPut("{id}")]
    public void Put(int id, [FromBody]string value)
    {
    }
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
  }
}
