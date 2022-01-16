using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  // container for stripe customer info
  public class StripeCustomerDocument : Document
  {
    [JsonProperty(PropertyName = "customerId")]
    public string StripeCustomerId { get; set; }

    [JsonProperty(PropertyName = "utcCreated")]
    public DateTime UtcCreated { get; set; }

    public StripeCustomerDocument()
    {
      EntityType = "StripeCustomer";
    }
  }
}
