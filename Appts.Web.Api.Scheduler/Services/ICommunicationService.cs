using Appts.Models.SendGrid.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Api.Scheduler.Services
{
  public interface ICommunicationService
  {
    #region "Appointments"
    Task<bool> SendApptScheduledAsync(SendApptScheduledRequest request);
    Task<bool> SendApptCanceledAsync(SendApptScheduledRequest request);
    Task<List<long>> SendApptReminderSmsAsync(SendApptReminderRequest request);
    Task<int> CanceledScheduledSmsLists(List<long> clientApptReminders, List<long> spApptReminders);
    #endregion
    #region "Client"
    Task<bool> SendClientInviteAsync(SendClientInviteRequest request);
    #endregion
    #region "Subscription"
    Task<bool> SendWelcomeEmailAsync(SendWelcomeRequest request);
    Task<long> SendExpiringSoonEmailAsync(SendTrialExpiringRequest request);
    Task<long> SendTrialExpiredEmailAsync(SendTrialExpiringRequest request);
    Task<bool> SendCanceledFreeTrialEmailAsync(SendTrialExpiringRequest request);
    Task<bool> SendCanceledSubscriptionEmailAsync(SendTrialExpiringRequest request);
    Task<bool> SendThankYouForJoiningEmailAsync(SendWelcomeRequest request);
    #endregion

  }
}
