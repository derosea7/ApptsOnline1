using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class ViewAppointmentsResponse
  {
    public string IanaTimeZoneId { get; set; }
    public string SpVanityUrl { get; set; }
    public List<Appointment> Appointments { get; set; }
    public List<AppointmentType> AppointmentTypes { get; set; }

    //count of appointments being marked as ready
    public int NewAppointments { get; set; }
    public List<string> NewApptIds { get; set; }
  }
}
