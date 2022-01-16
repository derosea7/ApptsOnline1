using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Appts.Models.Api
{
  public class WhitelistApiModel
  {
    [JsonProperty(PropertyName = "emails")]
    public List<string> Emails { get; set; }
  }
}
