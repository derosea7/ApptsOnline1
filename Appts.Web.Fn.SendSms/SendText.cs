using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Appts.Models.Sms;
using Newtonsoft.Json;
using System.Text;
namespace Appts.Web.Fn.SendSms
{
  public static class SendText
  {
    [FunctionName("SendText")]
    public static void Run([ServiceBusTrigger("apptssms", Connection = "ServiceBusConnStr")] string myQueueItem, ILogger log)
    {
      log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
      string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
      string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
      SendSmsRequest sms;
      try
      {
        JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
        serializerSettings.TypeNameHandling = TypeNameHandling.Auto;
        serializerSettings.NullValueHandling = NullValueHandling.Ignore;
        serializerSettings.Formatting = Formatting.Indented;
        sms = JsonConvert.DeserializeObject<SendSmsRequest>(myQueueItem, serializerSettings);
      }
      catch (Exception ex)
      {
        log.LogError("Failed to deserialize sms; azure fn", ex.Message, ex.StackTrace);
        throw;
      }
      try
      {
        TwilioClient.Init(accountSid, authToken);
        var message = MessageResource.Create(
            body: sms.TextMsg,
            from: new Twilio.Types.PhoneNumber("+12057497723"),
            to: new Twilio.Types.PhoneNumber($"+{sms.CountryCode}{sms.ToPhoneNumber}")
        );
      }
      catch (Exception ex)
      {
        log.LogError("Failed to initialize or send twilio message: ", ex.Message, ex.StackTrace);
        throw;
      }
    }
  }
}