using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class AuditTrail
  {
    [JsonProperty(PropertyName = "on")]
    public DateTime? On { get; set; }
    //ui display info, convert on to time ago liek 1 hr ago or someting
    [JsonIgnore]
    public string OnAsTimeAgo { get; set; }
    [JsonProperty(PropertyName = "byId")]
    public string ById { get; set; }
    [JsonProperty(PropertyName = "byName")]
    public string ByName { get; set; }
    [JsonProperty(PropertyName = "byEmail")]
    public string ByEmail { get; set; }
  }
}
