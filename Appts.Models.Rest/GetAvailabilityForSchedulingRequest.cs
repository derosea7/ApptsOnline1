using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class GetAvailabilityForSchedulingRequest
  {
    public string ServiceProviderId { get; set; }
    public string AppointmentTypeId { get; set; }
    public DateTime StartDateRange { get; set; }

    // if true, process the request for only 7 days from
    // start date range.
    // if false, bring back all selectable dates and
    // the first available week
    public bool OneWeekOnly { get; set; } = true;
  }
}
