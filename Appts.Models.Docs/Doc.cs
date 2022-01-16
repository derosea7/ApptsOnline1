using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Docs
{
  public class Doc : IDoc
  {
    [JsonProperty(PropertyName = "type")]
    public string EType { get; set; }
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "pk")]
    public string PartitionKey { get; set; }
  }
}
