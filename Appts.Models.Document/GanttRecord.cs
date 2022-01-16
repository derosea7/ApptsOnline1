using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Document
{
  public class GanttRecord
  {
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("fromDate")]
    public string FromDate { get; set; }
    [JsonProperty("toDate")]
    public string ToDate { get; set; }
    [JsonProperty("color")]
    public string Color { get; set; }
  }
}
