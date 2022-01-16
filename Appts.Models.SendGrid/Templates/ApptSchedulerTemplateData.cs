using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Templates
{
  public class ApptSchedulerTemplateData : IDynamicTemplateData
  {
    [JsonProperty("summary")]
    public string ApptSummary { get; set; }
    //[JsonProperty("client_email")]
    //public string ClientEmail { get; set; }
    //[JsonProperty("sp_email")]
    //public string SpEmail { get; set; }
    //[JsonProperty("personal_message")]
    //public string PersonalMessage { get; set; }

    [JsonProperty("cancel")]
    public string Cancel { get; set; }

    [JsonProperty("reschedule")]
    public string Reschedule { get; set; }

    [JsonProperty("scheduledBy")]
    public string ScheduledBy { get; set; }

    [JsonProperty("vanity")]
    public string Vanity { get; set; }


    [JsonProperty("notes")]
    public string Notes { get; set; }

    // used during cancel

    [JsonProperty("cancelNotes")]
    public string CancelNotes { get; set; }
    [JsonProperty("canceledBy")]
    public string CanceledBy { get; set; }
  }
}
