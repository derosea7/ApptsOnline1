using Appts.Models.Document;
using Appts.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Api.Scheduler.Repositories
{
  public interface ISubscriptionRepository
  {
    Task<string> GetActivePlanAsync(string userId);
    Task<SubscriptionDocument> GetActiveSubscriptionAsync(string userId);
    Task CancelAsync(SubscriptionDocument subscription);
    Task CreateTrailSubscription(CreateTrialSubscriptionRequest request);
    Task CreateTrialPromo(string userId, List<long> sequenceNumbers);
    Task<bool> HasAfterTrialPromoAsync(string userId);
    Task CreateStripeCustomerAsync(CreateStripeCustomerRequest request);
    Task ConvertTrialToPaidAsync(string serviceProviderId);
    Task CreateStripeEventAsync(string eventType);
  }
}
