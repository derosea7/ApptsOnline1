using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Rest;
using Microsoft.AspNetCore.Mvc;
using Appts.Web.Api.Scheduler.Repositories;
using Appts.Messaging.ServiceBus;
using Appts.Models.Domain;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Appts.Models.SendGrid;
using Appts.Models.SendGrid.Requests;
using Appts.Web.Api.Scheduler.Services;

namespace Appts.Web.Api.Scheduler.Controllers
{
  [Authorize]
  [Route("api/[controller]/[action]")]
  public class SubscriptionController : Controller
  {
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly IClientRepository _clientRepository;
    private readonly ICommunicationService _comm;
    public SubscriptionController(
      ISubscriptionRepository subscriptionRepository,
            IServiceProviderRepository serviceProviderRepository,
      IClientRepository clientRepository, ICommunicationService comm)
    {
      _subscriptionRepository = subscriptionRepository;
      _serviceProviderRepository = serviceProviderRepository;
      _clientRepository = clientRepository;
      _comm = comm;
    }
    #region "HTTP calls"
    /// <summary>
    /// Retrieve the claim to a subscription for a user if it exists.
    /// Called by IEF in Azure B2C when User signs in.
    /// Note: The output of this is used all over the website to determine
    /// actions (what to show, what data to collect, etc.).
    /// </summary>
    /// <param name="request">
    /// Request from Azure B2C IEF User Journey with users Object Id, 
    /// which can be looked up in our db to see if they have a subscription document.
    /// </param>
    /// <returns>
    /// The key claim used to determine a users subscription access levels.
    /// This output is peristed in User.Claims.
    /// </returns>
    [HttpPost]
    public GetSubscriptionPlanResponse GetActivePlan([FromBody] GetSubscriptionPlanRequest request)
    {
      var response = new GetSubscriptionPlanResponse();
      string plan = _subscriptionRepository.GetActivePlanAsync(request.ObjectId).GetAwaiter().GetResult();
      if (plan != null)
      {
        if (plan == "FreeTrial")
        {
          response.Ext_orgId = "Type=Sp,SubStat=Trial";
        }
        else if (plan == "Paid")
        {
          response.Ext_orgId = "Type=Sp,SubStat=Paid";
        }
        else if (plan == "Admin")
        {
          response.Ext_orgId = "Type=Sp,Type=Admin,SubStat=Paid";
        }
        else if (plan.StartsWith("Terminated"))
        {
          response.Ext_orgId = "Type=Client,SubStat=Terminated";
        }
      }
      else
      {
        // chance claim to tailor marketing to capture the fading lead.
        bool hasPromoFlag = _subscriptionRepository.HasAfterTrialPromoAsync(request.ObjectId)
          .GetAwaiter().GetResult();
        if (hasPromoFlag)
        {
          response.Ext_orgId = "Type=Client,SubStat=Promo";
        }
      }
      if (response.Ext_orgId == null)
      {
        response.Ext_orgId = "Type=Client";
      }
      return response;
    }
    [HttpPost]
    public void CreateStripeCustomer([FromBody] CreateStripeCustomerRequest request)
    {
      _subscriptionRepository.CreateStripeCustomerAsync(request).GetAwaiter().GetResult();
      _subscriptionRepository.ConvertTrialToPaidAsync(request.UserId).GetAwaiter().GetResult();
    }
    [HttpPost]
    public CreateTrialSubscriptionResponse CreateTrialSubscription([FromBody]CreateTrialSubscriptionRequest request)
    {
      var response = new CreateTrialSubscriptionResponse();
      _subscriptionRepository.CreateTrailSubscription(request)
        .GetAwaiter().GetResult();
      response.Ext_orgId = "FreeTrial";
      SendWelcomeEmail(request);
      long snExpiringSoon1 = ScheduleExpiringSoon1(request.GivenName, request.Surname, request.Email);
      long snExpired1 = ScheduleExpired1(request.GivenName, request.Surname, request.Email);
      List<long> seqNo = new List<long>();
      seqNo.Add(snExpiringSoon1);
      seqNo.Add(snExpired1);
      //add more to seqNo here, to send more emails in the series.
      _subscriptionRepository.CreateTrialPromo(request.ObjectId, seqNo).GetAwaiter().GetResult();
      var createClientRequest = new CreateClientRequest()
      { 
        DisplayName = request.DisplayName,
        Email = request.Email,
        GivenName = request.GivenName,
        ObjectId = request.ObjectId,
        Surname = request.Surname
      };
      _clientRepository.CreateIfNotExistsAsync(createClientRequest).GetAwaiter().GetResult();
      return response;
    }
    /// <summary>
    /// Cancel a Service Provider's Subscription.
    /// Handles free trials and paid trials differently. Sends email for each scenario.
    /// </summary>
    /// <param name="request">Contains service provider user id to cancel active subscription for.</param>
    [HttpPatch]
    public void Cancel([FromBody]CancelSubscriptionRequest request)
    {
      var subscription = _subscriptionRepository.GetActiveSubscriptionAsync(request.ServiceProviderId)
        .GetAwaiter().GetResult();
      _subscriptionRepository.CancelAsync(subscription).GetAwaiter().GetResult();
      //send cancelation email here
      var sp = _serviceProviderRepository.GetAsync(request.ServiceProviderId).GetAwaiter().GetResult();
      string displayName = sp.DisplayName;
      string email = sp.Email;
      if (subscription.Plan == "TerminatedPaid")
      {
        SendCanceledPayedSubscriptionEmail(displayName, email);
      }
      else if (subscription.Plan == "TerminatedFreeTrial")
      {
        SendCanceledFreeTrialEmail(displayName, email);
      }
    }
    [HttpGet]
    public GetSubscriptionDetailResponse GetActivePlanDetail(GetSubscriptionDetailRequest request)
    {
      var subscription = _subscriptionRepository.GetActiveSubscriptionAsync(request.ServiceProviderId)
        .GetAwaiter().GetResult();
      return new GetSubscriptionDetailResponse()
      {
        EffectiveDate = subscription.EffectiveDate,
        ExpirationDate = subscription.ExpirationDate,
        Plan = subscription.Plan,
        TerminationDate = subscription.TerminationDate
      };
    }
    #endregion
    #region "Service Bus to Azure Fn to SendGrid stuff"
    void SendWelcomeEmail(CreateTrialSubscriptionRequest request)
    {
      var emailRequest = new SendWelcomeRequest()
      {
        CustomerEmail = request.Email,
        CustomerFirstName = request.GivenName,
        CustomerLastName = request.Surname
      };
      _comm.SendWelcomeEmailAsync(emailRequest);
    }
    long ScheduleExpiringSoon1(string firstName, string lastName, string email)
    {
      var request = new SendTrialExpiringRequest()
      { 
        Email = email,
        FirstName = firstName,
        LastName = lastName
      };
      return _comm.SendExpiringSoonEmailAsync(request).GetAwaiter().GetResult();
    }
    long ScheduleExpired1(string firstName, string lastName, string email)
    {
      var request = new SendTrialExpiringRequest()
      {
        Email = email,
        FirstName = firstName,
        LastName = lastName
      };
      return _comm.SendTrialExpiredEmailAsync(request).GetAwaiter().GetResult();
    }
    void SendCanceledFreeTrialEmail(string firstName, string email)
    {
      var request = new SendTrialExpiringRequest()
      {
        Email = email,
        FirstName = firstName,
        LastName = "lastname"
      };
      _comm.SendCanceledFreeTrialEmailAsync(request).GetAwaiter().GetResult();
    }
    void SendCanceledPayedSubscriptionEmail(string firstName, string email)
    {
      var request = new SendTrialExpiringRequest()
      {
        Email = email,
        FirstName = firstName,
        LastName = "lastname"
      };
      _comm.SendCanceledSubscriptionEmailAsync(request).GetAwaiter().GetResult();
    }

    #endregion
  }
}