using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class LookupDocument : Document
  {
    [JsonProperty(PropertyName = "k")]
    public string Key { get; set; }

    [JsonProperty(PropertyName = "v")]
    public string Value { get; set; }

    public LookupDocument()
    {
      EntityType = "Lookup";
    }
  }
}
