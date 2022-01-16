using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document.GoogleSearch
{
  public class SearchTrendsRaw : Document
  {
    public SearchTrendsRaw()
    {
      EntityType = "SearchTrendGroupRaw";
      //UserId = $"SearchTrendGroupRaw_{DateTime.UtcNow.ToString("yyyy-MM-dd")}";
      UserId = "SearchTrendGroupRaw";
      Id = Guid.NewGuid().ToString();
    }
    [JsonProperty("columns")]
    public string[] Columns { get; set; }

    [JsonProperty("symbols")]
    public string[] Symbols { get; set; }

    //[JsonProperty("index")]
    //public long[] DateAsSecondsUtc { get; set; }

    //[JsonProperty("index")]
    //public string[] DateAsSecondsUtc { get; set; }

    [JsonProperty("index")]
    public DateTime[] DateString { get; set; }

    //[JsonProperty("data")]
    ////public List<byte[]> Interest { get; set; }
    //public List<List<int>> Interest { get; set; }

    [JsonProperty("data")]
    //public List<byte[]> Interest { get; set; }
    public int[] Interest { get; set; }

    [JsonProperty("latestTime")]
    public DateTime LatestTime { get; set; }
  }
}
