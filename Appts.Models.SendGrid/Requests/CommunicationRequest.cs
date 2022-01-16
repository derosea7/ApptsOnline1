using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Requests
{
  public class CommunicationRequest
  {
    public bool SendEmail { get; set; }
    public bool SendSms { get; set; }
  }
}
