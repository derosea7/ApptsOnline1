using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Messaging.ServiceBus;
using Appts.Models.SendGrid;
using Appts.Models.SendGrid.Requests;
using Appts.Models.SendGrid.Templates;
using Appts.Models.Sms;
using Newtonsoft.Json;
namespace Appts.Web.Api.Scheduler.Services
{
  public class CommunicationService : ICommunicationService
  {
    private readonly IBus _bus;
    public CommunicationService(IBus bus)
    {
      _bus = bus;
    }
    #region "Appointments"
    public async Task<bool> SendApptScheduledAsync(SendApptScheduledRequest request)
    {
      bool success = false;
      if (request.SendEmail)
      {
        success = await SendApptScheduledEmailAsync(request);
      }
      if (request.SendSms)
      {
        success = await SendApptScheduledSmsAsync(request);
      }
      return success;
    }
    public async Task<bool> SendApptScheduledEmailAsync(SendApptScheduledRequest request)
    {
      bool success = false;
      var emailRequest = EmailRequests.GetApptScheduledMessage(request);
      string json = SerializeSendEmailRequest(emailRequest);
      await _bus.SendMessageAsync(json);
      success = true;
      return success;
    }
    public async Task<bool> SendApptScheduledSmsAsync(SendApptScheduledRequest request)
    {
      bool success = false;
      //var emailRequest = EmailRequests.GetClientInviteMessage(request);
      //string json = SerializeSendEmailRequest(emailRequest);
      string msg = $"Appt scheduled: {request.ApptSummary} - Cancel: {request.CancelUrl} - Reply STOP to OptOut";
      //string msg = $"Appt scheduled: {request.ApptSummary} - Reply STOP to OptOut";
      var sms = new SendSmsRequest(request.ClientMobile, msg);
      var json = JsonConvert.SerializeObject(sms);
      await _bus.SendSmsMessageAsync(json);
      success = true;
      return success;
    }
    public async Task<bool> SendApptCanceledAsync(SendApptScheduledRequest request)
    {
      bool success = false;
      if (request.SendEmail)
      {
        success = await SendApptCanceledEmailAsync(request);
      }
      if (request.SendSms)
      {
        success = await SendApptCanceledSmsAsync(request);
      }
      return success;
    }
    public async Task<bool> SendApptCanceledEmailAsync(SendApptScheduledRequest request)
    {
      bool success = false;
      var emailRequest = EmailRequests.GetApptCanceledMessage(request);
      string json = SerializeSendEmailRequest(emailRequest);
      await _bus.SendMessageAsync(json);
      success = true;
      return success;
    }
    public async Task<bool> SendApptCanceledSmsAsync(SendApptScheduledRequest request)
    {
      bool success = false;
      //var emailRequest = EmailRequests.GetClientInviteMessage(request);
      //string json = SerializeSendEmailRequest(emailRequest);
      string msg = $"Appt canceled: {request.ApptSummary}";
      var sms = new SendSmsRequest(request.ClientMobile, msg);
      var json = JsonConvert.SerializeObject(sms);
      await _bus.SendSmsMessageAsync(json);
      success = true;
      return success;
    }
    //public async Task<CommunicationResponse> SendApptReminderAsync(SendApptReminderRequest request)
    //{
    //  var response = new CommunicationResponse();
    //  if (request.SendEmail)
    //  {
    //    //send only one email to client and cc sp
    //    response.EmailSequenceNumber = await SendApptReminderEmailAsync(request);
    //  }
    //  if (request.SendSms) // for client
    //  {
    //    //send 2 texts, one for client and one for sp
    //    response.SmsSequenceNumber = await SendApptReminderSmsAsync(request);
    //  }
    //  return response;
    //}
    public async Task<long> SendApptReminderEmailAsync(SendApptReminderRequest request)
    {

      return 1;
    }
    public async Task<List<long>> SendApptReminderSmsAsync(SendApptReminderRequest request)
    {
      var seqNum = new List<long>();
      if (request.Reminder1 != null)
      { 
        var reminder1Json = GetApptReminderSmsJson(request.ClientPhoneNumber, request.ApptSummary, "1 hour");
        seqNum.Add(await _bus.ScheduleCancelableSmsMessageAsync(reminder1Json, (DateTime)request.Reminder1));
      }
      if (request.Reminder2 != null)
      { 
        string reminder2Json = GetApptReminderSmsJson(request.ClientPhoneNumber, request.ApptSummary, "15 mins");
        seqNum.Add(await _bus.ScheduleCancelableSmsMessageAsync(reminder2Json, (DateTime)request.Reminder2));
      }
      return seqNum;
    }
    public async Task<int> CanceledScheduledSmsLists(List<long> clientApptReminders, List<long> spApptReminders)
    {
      int canceledCount = 0;
      if (clientApptReminders != null)
        canceledCount = await _bus.CancelScheduledSmsMessagesAsync(clientApptReminders);
      if (spApptReminders != null )
        canceledCount += await _bus.CancelScheduledSmsMessagesAsync(spApptReminders);
      return canceledCount;
    }
    string GetApptReminderSmsJson(string phoneNumber, string apptSummary, string timeUntilApptStarts)
    {
      string msg = $"Appt reminder | {timeUntilApptStarts} | {apptSummary}";
      var sms = new SendSmsRequest(phoneNumber, msg);
      return JsonConvert.SerializeObject(sms);
    }
    #endregion
    #region "Subscription"
    public async Task<bool> SendWelcomeEmailAsync(SendWelcomeRequest request)
    {
      bool success = false;
      var emailRequest = EmailRequests.GetWelcomeMessage(request);
      string json = SerializeSendEmailRequest(emailRequest);
      await _bus.SendMessageAsync(json);
      success = true;
      return success;
    }
    public async Task<long> SendExpiringSoonEmailAsync(SendTrialExpiringRequest request)
    {
      //prod
      DateTime scheduledTime = DateTime.UtcNow.AddDays(7);

      //testing
      //DateTime scheduledTime = DateTime.UtcNow.AddMinutes(7);
      var emailRequest = EmailRequests.GetExpiringSoonMessage(request);
      string json = SerializeSendEmailRequest(emailRequest);
      return await _bus.ScheduleCancelableMessageAsync(json, scheduledTime);
    }
    public async Task<long> SendTrialExpiredEmailAsync(SendTrialExpiringRequest request)
    {
      //prod
      DateTime scheduledTime = DateTime.UtcNow.AddDays(11);

      //testing
      //DateTime scheduledTime = DateTime.UtcNow.AddMinutes(20);
      var emailRequest = EmailRequests.GetTrialExpiredMessage(request);
      string json = SerializeSendEmailRequest(emailRequest);
      return await _bus.ScheduleCancelableMessageAsync(json, scheduledTime);
    }
    public async Task<bool> SendCanceledFreeTrialEmailAsync(SendTrialExpiringRequest request)
    {
      bool success = false;
      var emailRequest = EmailRequests.GetCanceledFreeTrialMessage(request);
      string json = SerializeSendEmailRequest(emailRequest);
      await _bus.SendMessageAsync(json);
      success = true;
      return success;
    }
    public async Task<bool> SendCanceledSubscriptionEmailAsync(SendTrialExpiringRequest request)
    {
      bool success = false;
      var emailRequest = EmailRequests.GetCanceledSubscriptionMessage(request);
      string json = SerializeSendEmailRequest(emailRequest);
      await _bus.SendMessageAsync(json);
      success = true;
      return success;
    }
    #endregion
    #region "Client"
    public async Task<bool> SendThankYouForJoiningEmailAsync(SendWelcomeRequest request)
    {
      bool success = false;
      var emailRequest = EmailRequests.GetThankYouForJoiningMessage(request);
      string json = SerializeSendEmailRequest(emailRequest);
      await _bus.SendMessageAsync(json);
      success = true;
      return success;
    }
    public async Task<bool> SendClientInviteAsync(SendClientInviteRequest request)
    {
      bool success = false;
      if (request.SendEmail)
      {
        success = await SendClientInviteEmailAsync(request);
      }
      if (request.SendSms)
      {
        success = await SendClientInviteSmsAsync(request);
      }
      return success;
    }
    public async Task<bool> SendClientInviteEmailAsync(SendClientInviteRequest request)
    {
      bool success = false;
      var emailRequest = EmailRequests.GetClientInviteMessage(request);
      string json = SerializeSendEmailRequest(emailRequest);
      await _bus.SendMessageAsync(json);
      success = true;
      return success;
    }
    public async Task<bool> SendClientInviteSmsAsync(SendClientInviteRequest request)
    {
      bool success = false;
      //var emailRequest = EmailRequests.GetClientInviteMessage(request);
      //string json = SerializeSendEmailRequest(emailRequest);
      string msg = $"{request.SpDisplayName} has invited to schedule an appointment! Use link {request.VanityUrl}";
      var sms = new SendSmsRequest(request.ClientPhoneNumber, msg);
      var json = JsonConvert.SerializeObject(sms);
      await _bus.SendSmsMessageAsync(json);
      success = true;
      return success;
    }
    #endregion
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
      serializer.TypeNameHandling = TypeNameHandling.Auto; //this is key
      serializer.Formatting = Formatting.Indented;
      return JsonConvert.SerializeObject(request, serializer);
    }

  }
}
