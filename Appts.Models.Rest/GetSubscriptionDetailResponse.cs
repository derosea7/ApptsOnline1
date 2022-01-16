using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class GetSubscriptionDetailResponse
  {
    public string Plan { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime? TerminationDate { get; set; }
  }
}
