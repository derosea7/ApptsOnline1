using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class SubscriptionDocument : Document
  {
    public SubscriptionDocument()
    {
      EntityType = "Subscription";
    }
    /// <summary>
    /// Date when subscription becomes active
    /// </summary>
    [JsonProperty(PropertyName = "effDate")]
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Date when subscription would naturally expire
    /// </summary>
    [JsonProperty(PropertyName = "expirationDate")]
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Populated if subscription is t erminated 
    /// before natural end
    /// </summary>
    [JsonProperty(PropertyName = "termDate")]
    public DateTime? TerminationDate { get; set; }

    [JsonProperty(PropertyName = "plan")]
    public string Plan { get; set; }
  }
}
