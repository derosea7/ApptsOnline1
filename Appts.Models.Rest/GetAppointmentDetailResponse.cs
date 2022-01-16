using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class GetAppointmentDetailResponse
  {
    public string IanaTimeZoneId { get; set; }
    public Appointment Appointment { get; set; }
  }
}
