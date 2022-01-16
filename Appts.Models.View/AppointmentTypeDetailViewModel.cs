using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.View
{
  public class AppointmentTypeDetailViewModel
  {
    public string Name { get; set; }
    public TimeSpan Duration { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    // not including recur in 1st release
    //// recur
    //[JsonConverter(typeof(StringEnumConverter))]
    //[JsonProperty(PropertyName = "recurRequirement")]
    //public RecurrenceRequirement RecurrenceRequirement { get; set; }

    // minimum time required
    public TimeSpan CancelationNotice { get; set; }

    // minimum time required
    public TimeSpan RescheduleNotice { get; set; }

    // minimum time required
    public TimeSpan MinimumNotice { get; set; }

    // minimum time required
    public TimeSpan MaximumNotice { get; set; }

    // prep time
    public TimeSpan BufferBefore { get; set; }

    // time allowed after appt before next
    public TimeSpan BufferAfter { get; set; }

  }
}
