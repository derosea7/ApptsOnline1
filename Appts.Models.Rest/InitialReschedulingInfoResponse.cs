using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class InitialReschedulingInfoResponse
  {
    public ServiceProviderDocument ServiceProvider { get; set; }
    public Appointment AppointmentToReschedule { get; set; }
  }
}
