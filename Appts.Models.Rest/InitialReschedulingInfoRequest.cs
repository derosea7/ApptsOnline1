using Appts.Models.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class InitialReschedulingInfoRequest
  {
    public string ServiceProviderVanityUrl { get; set; }
    public UserRole UserRole { get; set; }
    public string UserId { get; set; }
    // id of appt to reschedule
    public string AppointmentIdToReschedule { get; set; }

    
  }
}
