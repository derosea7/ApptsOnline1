using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Appts.Web.Api.Scheduler.Repositories;
using Appts.Models.Rest;

namespace Appts.Web.Api.Scheduler.Controllers
{
  [Route("api/[controller]/[action]")]
  public class OrganizationController : Controller
  {
    private readonly IOrganizationRepository _organizationRepository;
    public OrganizationController(IOrganizationRepository organizationRepository)
    {
      _organizationRepository = organizationRepository;
    }

    //10/31/19, backing off of org pardigm; will only have a service provider
    [HttpPost]
    public GetOrganizationIdentityResponse GetIdentity([FromBody]GetOrganizationIdentityRequest request)
    {
      //_organizationRepository.gettest
      var response = new GetOrganizationIdentityResponse()
      {
        Ext_orgId = _organizationRepository
          .GetOrganizationIdenity(request.ObjectId).GetAwaiter().GetResult()
      };
      return response;
    }

    // GET api/<controller>/5
    [HttpGet("{id}")]
    public string Get(string id)
    {
      var test = _organizationRepository.GetTestAsync()
        .GetAwaiter().GetResult();
      return test.TestValue;
      //return "value";
    }

  }
}
