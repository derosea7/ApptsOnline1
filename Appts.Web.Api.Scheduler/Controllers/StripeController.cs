using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.IO;
using Stripe.Checkout;
using Appts.Web.Api.Scheduler.Repositories;
using Microsoft.Extensions.Logging;
using Appts.Models.SendGrid;
using Newtonsoft.Json;
using Appts.Messaging.ServiceBus;
using Appts.Models.SendGrid.Templates;
namespace Appts.Web.Api.Scheduler.Controllers
{
  [Route("api/[controller]/[action]")]
  public class StripeController : Controller
  {
    private readonly ILookupRepository _lookupRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly ILogger _logger;
    private readonly IBus _bus;
    private static string _apptsAdminEmail = "admin@appts.online";
    const string webhookSigningSecret = "whsec_X8CyomMfrPjY3hij0vXBRpSPVbjfzllc";
    //const string webhookSigningSecret = "whsec_kUEsfDanCrN7Oaf1i5h5ozmXFy3JvUkI";
    public StripeController(
      ILookupRepository lookupRepository,
      ISubscriptionRepository subscriptionRepository,
      IServiceProviderRepository serviceProviderRepository,
      ILogger<StripeController> logger,
      IBus bus)
    {
      _lookupRepository = lookupRepository;
      _subscriptionRepository = subscriptionRepository;
      _serviceProviderRepository = serviceProviderRepository;
      _logger = logger;
      _bus = bus;
    }
    /// <summary>
    /// Webhook endpoint (not actually specific to completed checkout), that
    /// Stripe will direct event HTTP triggers to.
    /// 
    /// Using this specically to detemine when a checkout session is completed,
    /// but i think other events could be hooked-into here also.
    /// </summary>
    /// <returns>HTTP 200 status code if all is ok, bad request otherwise.</returns>
    [HttpPost]
    public IActionResult CompletedCheckout()
    {
      StripeConfiguration.ApiKey = "sk_test_hBGCYlxTGsZSTQiNyFIKBwon00sM3WieJs";
      //StripeConfiguration.ApiKey = "acct_1FGsb6FdSynE2tsT";
      var json = new StreamReader(HttpContext.Request.Body).ReadToEnd();
      try
      {
        var stripeEvent = EventUtility.ConstructEvent(json,
          Request.Headers["Stripe-Signature"], webhookSigningSecret);
        // hanlde the event
        if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
          var session = stripeEvent.Data.Object as Session;
          _logger.LogWarning("Handling checkout webhook for customer: {session.CustomerId}", session.CustomerId);
          // fulfill the purchase..
          FulfillSubscriptionPurchase(session);
          //session
          return Ok();
        }
        else
        {
          return Ok();
        }
      }
      catch (Exception ex)
      {
        return BadRequest();
      }
    }
    ////////////////[HttpPost]
    ////////////////public async Task<IActionResult> Webhook()
    ////////////////{
    ////////////////  var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
    ////////////////  Event stripeEvent;
    ////////////////  try
    ////////////////  {
    ////////////////    var webhookSecret = "STRIPE_WEBHOOOK_SECRET";
    ////////////////    stripeEvent = EventUtility.ConstructEvent(
    ////////////////      json,
    ////////////////      Request.Headers["Stripe-Signature"],
    ////////////////      webhookSecret);
    ////////////////  }
    ////////////////  catch (Exception ex)
    ////////////////  {

    ////////////////    return BadRequest();
    ////////////////  }
    ////////////////  switch (stripeEvent.Type)
    ////////////////  {
    ////////////////    case "checkout.session.completed":
    ////////////////      // payment successful, subscription creating
    ////////////////      // you should provision the subscription
    ////////////////      _subscriptionRepository.CreateStripeEventAsync("checkout.session.completed")
    ////////////////        .GetAwaiter().GetResult();
    ////////////////      break;
    ////////////////    case "invoice.paid":
    ////////////////      // continue to provider the subscription as payments continue to be made.
    ////////////////      // store teh status in your database and check when a user accesses your service.
    ////////////////      // this approach helps you avoid hitting rate limits.
    ////////////////      _subscriptionRepository.CreateStripeEventAsync("invoice.paid")
    ////////////////        .GetAwaiter().GetResult();
    ////////////////      break;
    ////////////////    case "invoice.payment_failed":
    ////////////////      // the payment failed or the customer does not have a valid payment method.
    ////////////////      // the subscription becomes past_due. notify your customer an dsend them to
    ////////////////      // the customer protal to update their payment information.
    ////////////////      _subscriptionRepository.CreateStripeEventAsync("invoice.payment_failed")
    ////////////////        .GetAwaiter().GetResult();
    ////////////////      break;
    ////////////////    default:
    ////////////////      // unhandled event type
    ////////////////      break;
    ////////////////  }
    ////////////////  return Ok();
    ////////////////}
    /// <summary>
    /// Business logic that must run after customer pays for full access and before
    /// customer can access their paid-for benefits.
    /// </summary>
    /// <param name="session">
    /// Stripe session object containing the subscription created by which customer using which payment intent
    /// </param>
    private void FulfillSubscriptionPurchase(Session session)
    {
      //_logger.LogInformation("FulfillSubscriptionPurchase(Session session), session.ToJson; " + session.ToJson());
      //_logger.LogInformation("FulfillSubscriptionPurchase(Session session), session.SubscriptionId; " + session.SubscriptionId);
      // do the thing
      string customerServiceProviderId = _lookupRepository
        .GetAsync("Lookup-StripeCustomerId-UserId", session.CustomerId)
        .GetAwaiter().GetResult();
      _logger.LogInformation("UserId from customer id: {customerServiceProviderId}", customerServiceProviderId);
      _subscriptionRepository.ConvertTrialToPaidAsync(customerServiceProviderId)
        .GetAwaiter().GetResult();
      var sp = _serviceProviderRepository.GetAsync(customerServiceProviderId).GetAwaiter().GetResult();
      SendThankYouEmail(sp.DisplayName, sp.Email);
      _logger.LogInformation("subscription successfully ran: _subscriptionRepository.ConvertTrialToPaidAsync(customerServiceProviderId)");
    }
    void SendThankYouEmail(string fname, string email)
    {

      var newSubTyRequest = GetThankYouNewSubscriberRequest(fname, email);
      string newSubTyJson = SerializeSendEmailRequest(newSubTyRequest);
      _bus.SendMessageAsync(newSubTyJson).GetAwaiter().GetResult();
    }
    /// <summary>
    /// Get the json that will be sent in Service Bus Message to trigger
    /// Azure Function to send SendMail HTTP Post request to SendGrid,
    /// which ultimately send email template with dynamic data
    /// </summary>
    /// <param name="request">This object will contain the details of where to send email and include dynamic template data.</param>
    /// <returns>string of Json that can be sent in body of Service Bus message.</returns>
    static string SerializeSendEmailRequest(SendMailRequest request)
    {
      JsonSerializerSettings serializer = new JsonSerializerSettings();
      serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
      serializer.NullValueHandling = NullValueHandling.Ignore;
      serializer.TypeNameHandling = TypeNameHandling.Auto;
      serializer.Formatting = Formatting.Indented;
      return JsonConvert.SerializeObject(request, serializer);
    }
    static SendMailRequest GetThankYouNewSubscriberRequest(string firstName, string email)
    {
      return new SendMailRequest()
      {
        Personalizations = new List<Personalizations>()
        {
          new Personalizations()
          {
            ToList = new List<To>()
            {
              new To(email, $"{firstName}")
            },
            Subject = "Thank You for Subscribing!",
            DynamicTemplateData = new WelcomeEmailTemplateData()
            {
              FirstName = firstName
            }
          }
        },
        From = new To(_apptsAdminEmail, "Appts Online"),
        ReplyTo = new To(_apptsAdminEmail, "Appts Online Administration"),
        TemplateId = "d-c0729dca10224cb480b080da3c7588e4"
      };
    }
  }
}