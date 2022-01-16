using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.View
{
  public class CancelAppointmentViewModel
  {
    public string IanaTimeZoneId { get; set; }
    public Appointment Appointment { get; set; }

    public string CancelationNotes { get; set; }
    public string AppointmentId { get; set; }
    public string SpVanityUrl { get; set; }
  }
}
