using Appts.Dal.Cosmos;
using Appts.Models.Document;
using Appts.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Common.Constants;
using Microsoft.Extensions.Logging;
namespace Appts.Web.Api.Scheduler.Repositories
{
  /// <summary>
  /// Responsible for creating and maintaining a valid
  /// subscription to the website.
  /// 
  /// To be a valid Service Provider, you must have a 
  /// valid and active Subscription document. Therefor, the creation
  /// of this document during the try-free sign-up phase is crucial,
  /// and has logging built around it.
  /// 
  /// The logs can be viewed in blog storage using Storage Explorer.
  /// </summary>
  public class SubscriptionRepository : ISubscriptionRepository
  {
    private readonly IDb _db;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly ILookupRepository _lookupRepository;
    private readonly ILogger _logger;
    private static byte trialLength = 10; //days
    public SubscriptionRepository(
      IDb db, 
      IServiceProviderRepository serviceProviderRepository,
      ILookupRepository lookupRepository,
      ILogger<SubscriptionRepository> logger)
    {
      _db = db;
      _serviceProviderRepository = serviceProviderRepository;
      _lookupRepository = lookupRepository;
      _logger = logger;
    }
    /// <summary>
    /// Converts trial user to paying user.
    /// TODO: delete trial promo record that triggers trial emails to be sent.
    /// </summary>
    /// <param name="serviceProviderId">User becoming SP.</param>
    /// <returns>Nothing if successful</returns>
    public async Task ConvertTrialToPaidAsync(string serviceProviderId)
    {
      var subscrption = await GetActiveSubscriptionAsync(serviceProviderId);
      subscrption.EffectiveDate = DateTime.UtcNow;
      subscrption.ExpirationDate = DateTime.UtcNow.AddYears(1);
      subscrption.TerminationDate = null;
      subscrption.Plan = "Paid";
      await _db.ReplaceNoReturnAsync(subscrption);
      // cleanup trial stuff as the user is now a paying subscriber.
      try
      {
        await DeleteTrialPromo(serviceProviderId);
      }
      catch (Exception ex)
      {
        _logger.LogInformation("Exception when trying to ");
      }
    }
    //public async Task CancelAsync(string userId)
    /// <summary>
    /// Update Subscription doc so it is considered canceled or terminated.
    /// </summary>
    /// <param name="subscription">The subscirption doc to replace.</param>
    /// <returns>Nothing if successful, exception otherwise.</returns>
    public async Task CancelAsync(SubscriptionDocument subscription)
    {
      string newPlan = "Terminated";
      if (subscription.Plan == "Paid")
      {
        newPlan += subscription.Plan;
      }
      else if (subscription.Plan == "FreeTrial")
      {
        newPlan += subscription.Plan;
      }
      subscription.TerminationDate = DateTime.UtcNow.AddHours(1);
      subscription.Plan = newPlan;
      await _db.ReplaceNoReturnAsync(subscription);
    }
    /// <summary>
    /// Deletes the trial promotional record that drives promo emails
    /// and meesages on website for user.
    /// TODO: cancel ScheduledEmailMessageSequenceNumbers for promo emails.
    /// </summary>
    /// <param name="serviceProviderId">Target service provider</param>
    /// <returns>Nothing if successful</returns>
    public async Task DeleteTrialPromo(string serviceProviderId)
    {
      string sql = $@"
        SELECT c.id
        FROM c
        where
            c.entityType = 'TrialPromo'
            and c.userId = @userId
      ";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", serviceProviderId)
      };
      var trialPromoId = await _db.GetSingleAsync<string>(sql, paramaters);
      await _db.DeleteNoReturnAsync<TrialPromotionDocument>(trialPromoId, serviceProviderId);
    }
    /// <summary>
    /// Get an active subscription for a given user id.
    /// </summary>
    /// <param name="userId">User Id to find active subscription for.</param>
    /// <returns>Subscription doc if found, else null.</returns>
    public async Task<SubscriptionDocument> GetActiveSubscriptionAsync(string userId)
    {
      string sql = $@"
      select * from (
          -- filter first as udf in where prevent usage of index 
          select *
          from c  
          where 
              c.userId = @userId
              and c.entityType = 'Subscription'
      ) a                                                     
      where                                                   
      udf.notExpired(a.expirationDate) = true                 
      and udf.notTerminated(a.termDate) = true                
      ";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId)
      };
      return await _db.GetSingleAsync<SubscriptionDocument>(sql, paramaters);
    }
    /// <summary>
    /// Returns name of the active plan or null if not active
    /// subscription exists.
    /// </summary>
    /// <param name="userId">Id of user to find subscription for.</param>
    /// <returns>Name of active plan if exists, null otherwise</returns>
    public async Task<string> GetActivePlanAsync(string userId)
    {
      string sql = $@"
      select value a.plan from (
          -- filter first as udf in where prevent usage of index 
          select 
              c.userId
              , c.expirationDate
              , c.termDate
              , c.plan
          from c  
          where 
              c.userId = @userId
              and c.entityType = 'Subscription'
      ) a
      where 
      udf.notExpired(a.expirationDate) = true
      and udf.notTerminated(a.termDate) = true
      ";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId)
      };
      return await _db.GetSingleAsync<string>(sql, paramaters);
    }
    public async Task<bool> HasAfterTrialPromoAsync(string userId)
    {
      string sql = $@"
        SELECT value c.id 
        FROM c
        where 
            c.userId = @userId
            and c.entityType = 'AfterTrialPromo'
        ";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId)
      };
      string promoId = await _db.GetSingleAsync<string>(sql, paramaters);
      return promoId != null ? true : false;
    }
    /// <summary>
    /// Create and save the neccessary records to enable a registered AD user
    /// to also be considered a Service Provider, on a Free Trial.
    /// </summary>
    /// <param name="request">Contains info required to init trial.</param>
    /// <returns>Nothing if successful, exception otherwise.</returns>
    public async Task CreateTrailSubscription(CreateTrialSubscriptionRequest request)
    {
      _logger.LogInformation("CreateTrialSubscription for: " + request.Email);
      var trial = new SubscriptionDocument()
      {
        UserId = request.ObjectId,
        EffectiveDate = DateTime.UtcNow,
        ExpirationDate = DateTime.UtcNow.AddDays(trialLength),
        Plan = "FreeTrial",
        Id = Guid.NewGuid().ToString()
      };
      await _db.CreateNoReturnAsync(trial);
      // before adding sp doc, check to see if vanity url is taken
      // if it is, append a random number
      string vanityUrl = request.DisplayName;
      var sp1 = new ServiceProviderDocument()
      {
        VanityUrl = vanityUrl
      };
      sp1.CleanVanityUrl();
      vanityUrl = sp1.VanityUrl;
      string userIdForVanityUrl = await _lookupRepository.GetAsync(EntityK.VanityUrlLookup, vanityUrl);
      if (!string.IsNullOrEmpty(userIdForVanityUrl))
      {
        // display name taken; append random number
        int rand = new Random().Next(1000);
        // could check again--maybe in future if needed
        vanityUrl = vanityUrl + rand;
      }
      var sp = new ServiceProviderDocument()
      {
        Id = Guid.NewGuid().ToString(),
        UserId = request.ObjectId,
        Email = request.Email,
        DisplayName = request.DisplayName,
        FirstName = request.GivenName,
        LastName = request.Surname,
        VanityUrl = vanityUrl,
        SchedulingPrivacyLevel = Models.Domain.SchedulingPrivacyLevel.AllowAnonymous
      };
      await _serviceProviderRepository.Create(sp);
      _logger.LogInformation("Service Provider created: " + sp.Email);
      //also need to add this vanity url (which we setting from display name here for default)
      //to lookup for when appointments are scheduled
      await _lookupRepository.PostAsync(EntityK.VanityUrlLookup, vanityUrl, request.ObjectId);
      // add default appointment type to get client started
      var defaultAppointmentType = new AppointmentType()
      {
        UserId = request.ObjectId,
        Id = Guid.NewGuid().ToString(),
        Name = "Consultation",
        Description = "Introductory meeting",
        Duration = new TimeSpan(0, 30, 0),
        Location = "-1",
        IsActive = true
      };
      await _db.CreateNoReturnAsync(defaultAppointmentType);
      _logger.LogInformation("Default appt type created for: " + sp.Email);
    }
    public async Task CreateTrialPromo(string userId, List<long> sequenceNumbers)
    {
      DateTime now = DateTime.UtcNow;
      DateTime end = DateTime.UtcNow.AddDays(21);
      TimeSpan ttl = end - now;
      var trialPromo = new TrialPromotionDocument()
      {
        Id = Guid.NewGuid().ToString(),
        UserId = userId,
        ttl = Math.Ceiling(ttl.TotalSeconds),
        ScheduledEmailMessageSequenceNumbers = sequenceNumbers
      };
      await _db.CreateNoReturnAsync(trialPromo);
    }
    public async Task CreateStripeCustomerAsync(CreateStripeCustomerRequest request)
    {
      var customer = new StripeCustomerDocument()
      {
        Id = Guid.NewGuid().ToString(),
        UserId = request.UserId,
        StripeCustomerId = request.StripeCustomerId,
        UtcCreated = DateTime.UtcNow
      };
      await _db.CreateNoReturnAsync(customer);
    }
    public async Task CreateStripeEventAsync(string eventType)
    { 
      //var doc = new StripeEvent
    }
  }
}