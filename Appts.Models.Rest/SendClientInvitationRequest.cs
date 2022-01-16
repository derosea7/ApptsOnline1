using System;
using System.Collections.Generic;
using System.Text;
namespace Appts.Models.Rest
{
  public class SendClientInvitationRequest
  {
    public string SpEmail { get; set; }
    public string ClientEmail { get; set; }
    public string ClientPhoneNumber { get; set; }
    public string SpDisplayName { get; set; }
    public string SpVanityUrl { get; set; }
    public string PersonalMessage { get; set; }
    public bool SendSms { get; set; }
    public bool SendEmail { get; set; }
  }
}
