using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Appts.Common.Constants;
using Appts.Models.Rest;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class StripeController : Controller
  {
    private readonly IApiClient _apiClient;
    private TelemetryClient _telemetry;
    private ILogger<StripeController> _logger;
    public StripeController(
      IApiClient apiClient, 
      TelemetryClient telemetry,
      ILogger<StripeController> logger)
    {
      _apiClient = apiClient;
      _telemetry = telemetry;
      _logger = logger;
    }
    /// <summary>
    /// Called when customer clicks Pay Not / Upgrade / Subscribe.
    /// Initializes / triggers Stripe to accept payment from this customer.
    /// User is directed to Stripe, then redirected back based on settings below.
    /// If a user clicks logo on payment page, the CancelUrl is called.
    /// </summary>
    /// <returns>Query string param indicating action and result taken on Stripe's page</returns>
    [HttpPost]
    public StartCheckoutSessionResponse StartCheckoutSession()
    {
      _telemetry.TrackEvent("StartCheckoutSession");
      // Set your secret key: remember to change this to your live secret key in production
      // See your keys here: https://dashboard.stripe.com/account/apikeys
      StripeConfiguration.ApiKey = "sk_test_hBGCYlxTGsZSTQiNyFIKBwon00sM3WieJs";
      string customerId = "";
      try
      {
        customerId = CreateCustomer(StripeConfiguration.ApiKey);
      }
      catch (Exception ex)
      {
        _logger.LogError("There was a problem creating Stripe customerId", ex.Message, ex.StackTrace);
        throw;
      }
      var response = new StartCheckoutSessionResponse();
      var options = new SessionCreateOptions
      {
        Customer = customerId,
        PaymentMethodTypes = new List<string>() { "card" },
        SubscriptionData = new SessionSubscriptionDataOptions
        {
          Items = new List<SessionSubscriptionDataItemOptions> 
          {
            new SessionSubscriptionDataItemOptions 
            {
                Plan = "plan_G6syY6MMe1Yfmd",
            }
          }
        },
        //SuccessUrl = "https://localhost:44352/Subscription/Success?session_id={CHECKOUT_SESSION_ID}",
        //SuccessUrl = "https://" + HttpContext.Request.Host.ToString() + "/Subscription/Index?success=t&session_id={CHECKOUT_SESSION_ID}",
        SuccessUrl = "https://" + HttpContext.Request.Host.ToString() + "/Subscription/Success?session_id={CHECKOUT_SESSION_ID}",
        CancelUrl = "https://" + HttpContext.Request.Host.ToString() + "/Subscription/Index?backout=t",
      };
      try
      {
        var service = new SessionService();
        Session session = service.Create(options);
        response.CheckoutSessionId = session.Id;
      }
      catch (Exception ex)
      {
        _logger.LogError("Exception encountered created Stripe Session", ex.Message, ex.StackTrace);
        throw;
      }
      return response;
    }
    public string CreateCustomer(string apiKey)
    {
      _telemetry.TrackEvent("CreateStripeCustomerInDb");
      StripeConfiguration.ApiKey = apiKey;
      string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      var options = new CustomerCreateOptions
      {
        Description = $"UserId: {userId}",
      };
      var service = new CustomerService();
      var customer = service.Create(options);
      // save customer id and user id to db for use in fulfilment
      var save = new CreateStripeCustomerRequest()
      {
        UserId = userId,
        StripeCustomerId = customer.Id
      };
      _apiClient.PostNoReturnAsync<CreateStripeCustomerRequest>(save, $"/api/Subscription/CreateStripeCustomer")
        .GetAwaiter().GetResult();
      // allows us to use stripe customer id to find user id when checkout completed
      // and all we have is customer id in webhook
      var lookup = new AddLookupRequest()
      {
        PartitionId = "Lookup-StripeCustomerId-UserId",
        Key = customer.Id,
        Value = userId
      };
      _apiClient.PostNoReturnAsync<AddLookupRequest>(lookup, $"/api/Lookup")
        .GetAwaiter().GetResult();
      return customer.Id;
    }
    // GET: /<controller>/
    public IActionResult Index()
    {
      return View();
    }
  }
}