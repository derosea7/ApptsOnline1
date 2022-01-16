using Appts.Models.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class GetSchedulingPrivacyLevelResponse
  {
    [JsonProperty(PropertyName = "serviceProviderId")]
    public string ServiceProviderId { get; set; }

    [JsonProperty(PropertyName = "serviceProviderEmail")]
    public string ServiceProviderEmail { get; set; }

    [JsonProperty(PropertyName = "foundServiceProvider")]
    public bool FoundServiceProvider { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "schedulingPrivacyLevel")]
    public SchedulingPrivacyLevel SchedulingPrivacyLevel { get; set; }
  }
}
