using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Document;
using Appts.Models.Domain;
using Newtonsoft.Json;

namespace Appts.Models.Rest
{
  public class CancelAppointmentRequest
  {
    // id of appointment to cancel
    [JsonProperty(PropertyName = "appointmentId")]
    public string AppointmentId { get; set; }
    // service provider
    [JsonProperty(PropertyName = "userId")]
    public string UserId { get; set; }

    [JsonProperty(PropertyName = "modified")]
    public Chronotope Modified { get; set; }

    [JsonProperty(PropertyName = "canceledOn")]
    public DateTime CanceledOn { get; set; }

    [JsonProperty(PropertyName = "canceledBy")]
    public string CanceledBy { get; set; }

    [JsonProperty(PropertyName = "canceledByName")]
    public string CanceledByName { get; set; }

    [JsonProperty(PropertyName = "cancelationNotes")]
    public string CancelationNotes { get; set; }

    [JsonProperty(PropertyName = "userTimeZoneId")]
    public string UserTimeZoneId { get; set; }
    //used in cancel emazil
    [JsonProperty(PropertyName = "spVanityUrl")]
    public string SpVanityUrl { get; set; }

    //just the vanity name, not entire url
    public string SpVanity { get; set; }
  }
}
