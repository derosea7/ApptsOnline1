using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Requests
{
  public class SendClientInviteRequest : CommunicationRequest
  {
    //  string spDisplayName, string email, string vanityUrl, string spEmail
    //  , string personalMessage = null)
    public string SpDisplayName { get; set; }
    public string ClientEmail { get; set; }
    //phone number to send text to
    public string ClientPhoneNumber { get; set; }
    public string VanityUrl { get; set; }
    public string SpEmail { get; set; }
    public string PersonalMessageFromSp { get; set; }
  }
}
