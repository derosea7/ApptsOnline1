using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Appts.Models.SendGrid;
using Microsoft.Azure.ServiceBus;
using RestSharp;
using Newtonsoft.Json;
using System.Text;

namespace Appts.Web.Fn.SendGridEmailV3
{
  public static class SendEmailByTemplate
  {
    [FunctionName("SendEmailByTemplate")]
    public static void Run([ServiceBusTrigger("email-reminders", Connection = "ServiceBusConnStr")] Message sendMailRequest, ILogger log)
    {
      //log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
      IRestResponse response;
      SendMailRequest sendMailObject;
      try
      {
        JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
        serializerSettings.TypeNameHandling = TypeNameHandling.Auto;
        serializerSettings.NullValueHandling = NullValueHandling.Ignore;
        serializerSettings.Formatting = Formatting.Indented;
        sendMailObject = JsonConvert.DeserializeObject<SendMailRequest>(
          Encoding.UTF8.GetString(sendMailRequest.Body), serializerSettings);
      }
      catch (Exception ex)
      {
        response = new RestResponse();
        response.StatusCode = System.Net.HttpStatusCode.BadRequest;
        response.ErrorMessage = "There was a problem trying to deserialize the request.";
        response.ErrorException = ex.InnerException;

        log.LogError("Failed to deserialize email; azure fn", ex.Message, ex.StackTrace);
        // for dev until i can figure out how to log in production
        throw;
      }
      var requestJson = JsonConvert.SerializeObject(sendMailObject);
      //testing sendgrid
      log.LogWarning(requestJson);

      var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
      var request = new RestRequest(Method.POST);
      request.AddHeader("content-type", "application/json");
      request.AddHeader("authorization", "Bearer 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");
      request.AddParameter("application/json", requestJson, ParameterType.RequestBody);

      try
      {
        response = client.Execute(request);
      }
      catch (Exception ex)
      {
        log.LogError("Azure Fn, client.Execute() failed", ex.Message, ex.StackTrace);
        ///throw until im sure logging works
        throw;
      }
    }
  }
}
