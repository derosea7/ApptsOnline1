using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document.GoogleSearch
{
  public class InterestOverTime
  {
    [JsonProperty("date")]
    public DateTime Date { get; set; }
    [JsonProperty("interest")]
    public byte Interest { get; set; }
  }
}
