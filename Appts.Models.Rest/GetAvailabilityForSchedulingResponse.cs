using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Api;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Models.Rest
{
  public class GetAvailabilityForSchedulingResponse
  {
    //public List<Appointment> Appointments { get; set; }
    //public List<AppointmentType> AppointmentTypes { get; set; }
    //public List<AvailabilityPeriod> AvailabilityPeriods { get; set; }

    // first week of availability found
    public AvailabilityPeriod Period { get; set; }

    // TODO: consider caching this
    public List<DateTime> DaysWithAvailability { get; set; }
  }
}
