using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Templates
{
  public class InviteClientTemplateData : IDynamicTemplateData
  {
    [JsonProperty("display_name")]
    public string DisplayName { get; set; }
    [JsonProperty("vanity_url")]
    public string VanityUrl { get; set; }
    [JsonProperty("personal_message")]
    public string PersonalMessage { get; set; }
  }
}
