using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class WhitelistDocument : Document
  {
    public WhitelistDocument()
    {
      EntityType = "Whitelist";
    }
    [JsonProperty(PropertyName = "emails")]
    public List<string> Emails { get; set; }
  }
}
