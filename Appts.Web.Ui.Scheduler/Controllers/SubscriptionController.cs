using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Common.Constants;
using Appts.Models.Domain;
using Appts.Models.Rest;
using Appts.Models.View;
using Appts.Web.Ui.Scheduler.Services;
using Microsoft.AspNetCore.Mvc;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  /// <summary>
  /// Manage a service provider's subscription to the site.
  /// </summary>
  public class SubscriptionController : Controller
  {
    private readonly IApiClient _apiClient;
    private readonly IHttpContextResolverService _httpContext;
    public SubscriptionController(IApiClient apiClient, IHttpContextResolverService httpContext)
    {
      _apiClient = apiClient;
      _httpContext = httpContext;
    }
    /// <summary>
    /// Show service provider their current subscription state.
    /// </summary>
    /// <returns>View of their current subscription state.</returns>
    public IActionResult Index(string success = null, string backout = null)
    {
      bool getFromSuccessfullPayment = false;
      if (success == "t")
        getFromSuccessfullPayment = true;

      if (TempData["CreatedPayingSubscription"]?.ToString() == bool.TrueString)
      {
        getFromSuccessfullPayment = true;
      }
      UserRole userRole = _httpContext.GetUserRole();
      var userId = _httpContext.GetUserId();
      ManageSubscriptionViewModel model = null;
      if (userRole == UserRole.Subscriber)
      { 
        // get current subscription info
        var response = _apiClient.GetAsync<GetSubscriptionDetailResponse>(
          $"/api/Subscription/GetActivePlanDetail?ServiceProviderId={userId}").GetAwaiter().GetResult();
        model = new ManageSubscriptionViewModel()
        {
          EffectiveDate = response.EffectiveDate,
          ExpirationDate = response.ExpirationDate,
          Plan = response.Plan,
          TerminationDate = response.TerminationDate
        };      
      }
      else if (userRole == UserRole.Client)
      {
        //model = new ManageSubscriptionViewModel()
        //{
          
        //};
      }
      var s = TempData["CreatedPayingSubscription"];
      return View(model);
    }
    [HttpPost]
    public IActionResult CancelSubscription()
    {
      var userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      var request = new CancelSubscriptionRequest() { ServiceProviderId = userId };
      _apiClient.PatchAsync<CancelSubscriptionRequest, Task>(request, $"/api/subscription/cancel")
        .GetAwaiter().GetResult();
      return RedirectToAction("Index");
    }
    public IActionResult Success()
    {
      ViewData["CreatedPayingSubscription"] = true;
      return RedirectToAction("Index");
      //return View();
    }
  }
}