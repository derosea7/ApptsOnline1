using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.SendGrid.Requests;
namespace Appts.Models.SendGrid.Templates
{
  public static class EmailRequests
  {
    private readonly static string _fromEmail = "notifications@appts.online";
    private readonly static string _fromEmailName = "Appts Online Notifications";
    //private readonly static string _replyToEmail = "noreply@appts.online";
    //private readonly static string _replyToEmailName = "Appts Online No Reply";
    private readonly static string _replyToEmail = "admin@appts.online";
    private readonly static string _replyToEmailName = "Appts Online Admin";
    /// <summary>
    /// Get request with from and reply to set. Optionally include template id.
    /// </summary>
    /// <param name="templateId">Optional; if included sets email template</param>
    /// <returns>SendMailRequest with from, to and optionally template id set.</returns>
    private static SendMailRequest GetBaseEmailRequest(string templateId = null)
    {
      var request = new SendMailRequest()
      {
        From = new To(_fromEmail, _fromEmailName),
        ReplyTo = new To(_replyToEmail, _replyToEmailName)
      };
      if (templateId != null)
        request.TemplateId = templateId;
      return request;
    }
    //Sendgrid will not send mail if to and cc is the same
    static void AddCcIfDifferentThanTo(SendMailRequest sendMailRequest,
      string toEmail, string ccEmail, string ccName)
    {
      if (toEmail != ccEmail)
      {
        sendMailRequest.Personalizations[0].CcList = new List<To>()
        {
          new To(ccEmail, ccName)
        };
      }
    }
    #region "Appointments"
    //todo: use base request, from to
    public static SendMailRequest GetApptScheduledMessage(SendApptScheduledRequest request)
    {
      var sendMail = GetBaseEmailRequest("d-2b496f20d59c434b8c35c0739826c505");
      sendMail.Personalizations = new List<Personalizations>()
      {
        new Personalizations()
        {
          ToList = new List<To>()
          {
            new To(request.ClientEmail, request.ClientName)
          },
          DynamicTemplateData = new ApptSchedulerTemplateData()
          {
              ApptSummary = request.ApptSummary,
              //Cancel = request.ApptId,
              Cancel = request.CancelUrl,
              //Reschedule = request.RescheduleUrl,
              Vanity = request.SpVanityUrl,
              ScheduledBy = request.ScheduleBy,
              Notes = request.Notes
          },
          SubscriptionPreferences = new UnsubscribeGroup()
          { 
            GroupId = 15423,
            GroupsToDisplay = new List<int>() { 15423 }
          }
        }
      };
      AddCcIfDifferentThanTo(sendMail, request.ClientEmail, request.SpEmail, request.SpName);
      return sendMail;
    }
    public static SendMailRequest GetApptCanceledMessage(SendApptScheduledRequest request)
    {
      var req = GetBaseEmailRequest("d-cffea93458144324bab594a9643550bd");
      req.Personalizations = new List<Personalizations>()
      {
        new Personalizations()
        {
          ToList = new List<To>()
          {
            new To(request.ClientEmail, $"{request.ClientName}")
          },
          DynamicTemplateData = new ApptSchedulerTemplateData()
          {
            ApptSummary = request.ApptSummary,
            Vanity = request.SpVanityUrl,
            CanceledBy = request.CanceledBy,
            CancelNotes = request.CancelationNotes
          },
          SubscriptionPreferences = new UnsubscribeGroup()
          {
            GroupId = 15423,
            GroupsToDisplay = new List<int>() { 15423 }
          }
        }
      };
      AddCcIfDifferentThanTo(req, request.ClientEmail, request.SpEmail, request.SpName);
      return req;
    }
    #endregion
    #region "Subscription"
    public static SendMailRequest GetWelcomeMessage(SendWelcomeRequest request)
    {
      var req = GetBaseEmailRequest("d-3f5785e9033c4789b6ae2257e88e4bb3");
      req.Personalizations = new List<Personalizations>()
      {
        new Personalizations()
        {
          ToList = new List<To>()
          {
            new To(request.CustomerEmail, $"{request.CustomerFirstName} {request.CustomerLastName}")
          },
          CcList = new List<To>()
          {
            new To("admin@appts.online", "Appts Admin")
          },
          DynamicTemplateData = new WelcomeEmailTemplateData()
          {
            FirstName = request.CustomerFirstName
          }
        }
      };
      return req;
    }
    public static SendMailRequest GetExpiringSoonMessage(SendTrialExpiringRequest request)
    {
      var req = GetBaseEmailRequest("d-e7838a2587f34bc8845e1fb88b6a79f7");
      req.Personalizations = new List<Personalizations>()
      {
        new Personalizations()
        {
          ToList = new List<To>()
          {
            new To(request.Email, $"{request.FirstName} {request.LastName}")
          },
          DynamicTemplateData = new WelcomeEmailTemplateData()
          {
            FirstName = request.FirstName
          }
        }
      };
      return req;
    }
    public static SendMailRequest GetTrialExpiredMessage(SendTrialExpiringRequest request)
    {
      var req = GetBaseEmailRequest("d-5c112c427f67413590aabb085db35314");
      req.Personalizations = new List<Personalizations>()
      {
        new Personalizations()
        {
          ToList = new List<To>()
          {
            new To(request.Email, $"{request.FirstName} {request.LastName}")
          },
          DynamicTemplateData = new WelcomeEmailTemplateData()
          {
            FirstName = request.FirstName
          }
        }
      };
      return req;
    }
    public static SendMailRequest GetCanceledFreeTrialMessage(SendTrialExpiringRequest request)
    {
      var req = GetBaseEmailRequest("d-f736600089f14cfa828b205feba6e49f");
      req.Personalizations = new List<Personalizations>()
      {
        new Personalizations()
        {
          ToList = new List<To>()
          {
            new To(request.Email, $"{request.FirstName} {request.LastName}")
          },
          DynamicTemplateData = new WelcomeEmailTemplateData()
          {
            FirstName = request.FirstName
          }
        }
      };
      return req;
    }
    public static SendMailRequest GetCanceledSubscriptionMessage(SendTrialExpiringRequest request)
    {
      var req = GetBaseEmailRequest("d-770e52cea4ca494197a41b3c91a8267c");
      req.Personalizations = new List<Personalizations>()
      {
        new Personalizations()
        {
          ToList = new List<To>()
          {
            new To(request.Email, $"{request.FirstName} {request.LastName}")
          },
          CcList = new List<To>()
          {
            new To("admin@appts.online", "Appts Online Customer Retention")
          },
          DynamicTemplateData = new WelcomeEmailTemplateData()
          {
            FirstName = request.FirstName
          }
        }
      };
      return req;
    }
    #endregion
    #region "Client"
    //d-ed96112a5d564de7a7e0525785bf741d
    public static SendMailRequest GetThankYouForJoiningMessage(SendWelcomeRequest request)
    {
      var req = GetBaseEmailRequest("d-ed96112a5d564de7a7e0525785bf741d");
      req.Personalizations = new List<Personalizations>()
      {
        new Personalizations()
        {
          ToList = new List<To>()
          {
            new To(request.CustomerEmail, $"{request.CustomerFirstName} {request.CustomerLastName}")
          },
          CcList = new List<To>()
          {
            new To("admin@appts.online", "Appts Admin")
          },
          DynamicTemplateData = new WelcomeEmailTemplateData()
          {
            FirstName = request.CustomerFirstName
          }
        }
      };
      return req;
    }
    public static SendMailRequest GetClientInviteMessage(SendClientInviteRequest request)
    {
      var req = GetBaseEmailRequest("d-1bc386dec681486a8387511feedc3eda");
      req.Personalizations = new List<Personalizations>()
      {
        new Personalizations()
        {
          ToList = new List<To>()
          {
            new To(request.ClientEmail, $"Client of {request.SpDisplayName}")
          },
          CcList = new List<To>()
          {
            new To(request.SpEmail, request.SpDisplayName)
          },
          DynamicTemplateData = new InviteClientTemplateData()
          {
            DisplayName = request.SpDisplayName,
            VanityUrl = request.VanityUrl,
            PersonalMessage = request.PersonalMessageFromSp
          }
        }
      };
      return req;
    }
    #endregion
  }
}
