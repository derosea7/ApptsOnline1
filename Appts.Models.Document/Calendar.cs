using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class Calendar : Document
  {
    public Calendar()
    {
      EntityType = "Calendar";
    }
    [JsonProperty(PropertyName = "active")]
    public bool Active { get; set; }
    [JsonProperty(PropertyName = "addedOn")]
    public DateTime AddedOn { get; set; }
    [JsonProperty(PropertyName = "revokedOn")]
    public DateTime? RevokedOn { get; set; }
    // google or ofice 365 calendars
    [JsonProperty(PropertyName = "provider")]
    public string Provider { get; set; }
  }
}
