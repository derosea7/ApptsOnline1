using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class RescheduleAppointmentRequest
  {
    public string ServiceProviderId { get; set; }
    public string AppointmentIdToReschedule { get; set; }
    public DateTime NewStartTime { get; set; }
    public DateTime NewEndTime { get; set; }
    public string RescheduleNotes { get; set; }
  }
}
