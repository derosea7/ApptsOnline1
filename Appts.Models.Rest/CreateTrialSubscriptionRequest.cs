using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class CreateTrialSubscriptionRequest
  {
    // user id from IEF
    public string ObjectId { get; set; }

    // addd name, surname, and wahtever
    public string DisplayName { get; set; }
    public string GivenName { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
  }
}
