using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class GetServiceProviderResponse
  {
    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName { get; set; }
    [JsonProperty(PropertyName = "vanityUrl")]
    public string VanityUrl { get; set; }
    [JsonProperty(PropertyName = "requireMyConfirmation")]
    public bool RequireMyConfirmation { get; set; }
    [JsonProperty(PropertyName = "schedulingPrivacyLevel")]
    public int SchedulingPrivacyLevel { get; set; }
    [JsonProperty(PropertyName = "timeZoneId")]
    public string TimeZoneId { get; set; }
    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }
    [JsonProperty(PropertyName = "mobilePhone")]
    public string MobilePhone { get; set; }
  }
}
