using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Requests
{
  public class SendWelcomeRequest
  {
    public string CustomerFirstName { get; set; }
    public string CustomerLastName { get; set; }
    public string CustomerEmail { get; set; }
  }
}
