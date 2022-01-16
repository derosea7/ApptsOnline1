using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Appts.Web;
using Appts.Models.Rest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Appts.Web.Api.Identity.Controllers
{
  //10/31/19, backing off of org pardigm; will only have a service provider
  [Route("api/[controller]/[action]")]
  public class IdentityController : Controller
  {
    private readonly ILogger _logger;
    private readonly IApiClient _apiClient;
    //private readonly IConfiguration _configuration;
    private readonly string _username;
    private readonly string _password;
    public IdentityController(IApiClient apiClient, IConfiguration configuration, ILogger<IdentityController> logger)
    {
      _logger = logger;
      _apiClient = apiClient;
      _username = configuration["Ief:Username"];
      _password = configuration["Ief:Password"];
    }

    // note: 10/31/2019, relenting to single provider model instead of
    // organization model. not method signature so no changes will be required as of now
    // to IEF
    [BasicAuthenticationFilter()]
    [HttpPost]
    public IActionResult GetOrganizationIdentity([FromBody]GetSubscriptionPlanRequest request)
    {
      string endpoint = $"/api/Subscription/GetActivePlan";
      var response = new GetSubscriptionPlanResponse();
      try
      {
        response = _apiClient.PostAsync<GetSubscriptionPlanRequest, GetSubscriptionPlanResponse>(
          request, endpoint
        ).GetAwaiter().GetResult();
      }
      catch (Exception ex)
      {
        //log
        _logger.LogError("error GetOrganizationIdentity, calling api-sched/sub/getActivePlan", ex.Message, ex.StackTrace);
        response.Ext_orgId = "Error";
      }
      if (response.Ext_orgId == null)
        response.Ext_orgId = "";
      return Ok(response);
    }
    // note: 10/31/2019, relenting to single provider model instead of
    // organization model. not method signature so no changes will be required as of now
    // to IEF
    [BasicAuthenticationFilter()]
    [HttpPost]
    public IActionResult CreateTrialSubscription([FromBody]CreateTrialSubscriptionRequest request)
    {
      string endpoint = $"/api/Subscription/CreateTrialSubscription";
      var response = new CreateTrialSubscriptionResponse();
      response = _apiClient.PostAsync<CreateTrialSubscriptionRequest, CreateTrialSubscriptionResponse>(
        request, endpoint
        ).GetAwaiter().GetResult();
      if (response.Ext_orgId == null)
        response.Ext_orgId = "";
      try
      {
        //result = response.OrganizationIdentity;
      }
      catch (Exception ex)
      {
        //log
      }
      return Ok(response);
    }
    /// <summary>
    /// Will be called during SUSI to provider users with Client documents.
    /// </summary>
    /// <param name="request">
    /// Will contain claims from IEF.
    /// </param>
    /// <returns>Output claim that IEF will collect.</returns>
    [BasicAuthenticationFilter()]
    [HttpPost]
    public IActionResult CreateClient([FromBody]CreateClientRequest request)
    {
      string endpoint = $"/api/Client/Create";
      var response = new CreateClientResponse();
      response = _apiClient.PostAsync<CreateClientRequest, CreateClientResponse>(
        request, endpoint).GetAwaiter().GetResult();
      if (response.Ext_orgId == null)
        response.Ext_orgId = "";
      try
      {

      }
      catch (Exception ex)
      {
        //log
        response.Ext_orgId = "Error";
        //throw;
      }
      return Ok(response);
    }
  }
}
