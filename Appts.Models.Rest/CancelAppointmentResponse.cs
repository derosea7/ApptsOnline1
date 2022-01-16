using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class CancelAppointmentResponse
  {
    //canceled appt, required to cancel gcal in ui soln
    public Appointment CanceledAppointment { get; set; }
    public bool CanceledApptSuccess { get; set; }
    public bool CanceledRemindersSuccess { get; set; }
    public bool CanceledGmailCalEvent { get; set; }
    public bool SentCancelMessages { get; set; }
    public string SpDisplayName { get; set; }
    public string SpEmail { get; set; }
  }
}
