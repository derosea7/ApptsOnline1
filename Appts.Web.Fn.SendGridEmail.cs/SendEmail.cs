using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;
using Appts.Models.SendGrid;
using Appts.Models.SendGrid.Templates;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using RestSharp;
namespace Appts.Web.Fn.SendGridEmail.cs
{
  /// <summary>
  /// Sends an email using SendGrid api v3. Email details are included in incoming message.
  /// 
  /// Note: message must be both serial/deserialized with specfic settings.
  /// </summary>
  public static class SendEmail
  {
    [FunctionName("SendEmail")]
    public static void Run([ServiceBusTrigger("email-reminders", Connection = "mybus")]Message sendMailRequest, ILogger log)
    {
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
        // for dev until i can figure out how to log in production
        throw;
        //return;
      }
      var requestJson = JsonConvert.SerializeObject(sendMailObject);
      var client = new RestClient("https://api.sendgrid.com/v3/mail/send");
      var request = new RestRequest(Method.POST);
      request.AddHeader("content-type", "application/json");
      request.AddHeader("authorization", "Bearer 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");
      request.AddParameter("application/json", requestJson, ParameterType.RequestBody);
      response = client.Execute(request);
    }
  }
}