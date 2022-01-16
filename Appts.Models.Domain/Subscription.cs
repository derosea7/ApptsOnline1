using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Domain
{
  public class Subscription
  {
    /// <summary>
    /// Date when subscription becomes active
    /// </summary>
    public DateTime EffectiveDate { get; set; }

    /// <summary>
    /// Date when subscription would naturally expire
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Populated if subscription is t erminated 
    /// before natural end
    /// </summary>
    public DateTime? TerminationDate { get; set; }
  }
}
