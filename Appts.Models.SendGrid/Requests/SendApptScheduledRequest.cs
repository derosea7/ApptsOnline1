using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Requests
{
  public class SendApptScheduledRequest : CommunicationRequest
  {
    public string ClientEmail { get; set; }
    public string ClientName { get; set; }
    public string ClientMobile { get; set; }
    public string SpEmail { get; set; }
    public string SpName { get; set; }
    //public string ApptId { get; set; }
    public string CancelUrl { get; set; }
    public string RescheduleUrl { get; set; }
    public string ApptSummary { get; set; }
    public string SpVanityUrl { get; set; }
    public string ScheduleBy { get; set; }
    public string Notes { get; set; }

    //used when cancleing
    public string CancelationNotes { get; set; }
    public string CanceledBy { get; set; }
  }
}
