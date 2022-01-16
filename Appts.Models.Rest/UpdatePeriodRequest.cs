using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class UpdatePeriodRequest
  {
    public string ServiceProviderId { get; set; }
    public string PeriodId { get; set; }

    // new values to update on existing document
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public List<Availability> AvailabilityDays { get; set; }
  }
}
