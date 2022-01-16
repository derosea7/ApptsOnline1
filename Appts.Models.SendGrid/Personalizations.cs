using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.SendGrid
{
  public class Personalizations
  {
    [JsonProperty(PropertyName = "to")]
    public List<To> ToList { get; set; }
    [JsonProperty(PropertyName = "cc")]
    public List<To> CcList { get; set; }
    [JsonProperty(PropertyName = "dynamic_template_data")]
    public IDynamicTemplateData DynamicTemplateData { get; set; }
    [JsonProperty(PropertyName = "subject")]
    public string Subject { get; set; }
    [JsonProperty(PropertyName = "asm")]
    public UnsubscribeGroup SubscriptionPreferences { get; set; }
  }
}
