using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.View
{
  public class ManageSubscriptionViewModel
  {
    public string Plan { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime? TerminationDate { get; set; }
  }
}
