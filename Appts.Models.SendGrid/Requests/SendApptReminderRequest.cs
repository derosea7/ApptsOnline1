using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Requests
{
  public class SendApptReminderRequest : CommunicationRequest
  {
    public string ClientEmail { get; set; }
    public string SpEmail { get; set; }
    public string ClientPhoneNumber { get; set; }
    public string SpPhoneNumber { get; set; }
    public string TimeUntilApptStarts { get; set; }
    public string ApptSummary { get; set; }
    //public string RescheduleLink { get; set; }
    public string DisplayTimeZoneId { get; set; }
    public DateTime? Reminder1 { get; set; }
    public DateTime? Reminder2 { get; set; }
  }
}
