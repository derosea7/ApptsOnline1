using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Requests
{
  public class SendTrialExpiringRequest
  {
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
  }
}
