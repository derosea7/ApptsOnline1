using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class SavePeriodRequest
  {
    public string ServiceProviderId { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public List<Availability> AvailabilityDays { get; set; }
  }
}
