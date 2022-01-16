using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.View
{
  public class AppointmentsByDateViewModel
  {
    public DateTime AppointmentDate { get; set; }
    public List<Appointment> Appointments { get; set; }
  }
}
