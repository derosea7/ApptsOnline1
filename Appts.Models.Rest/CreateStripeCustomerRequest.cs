using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class CreateStripeCustomerRequest
  {
    public string UserId { get; set; }
    public string StripeCustomerId { get; set; }
  }
}
