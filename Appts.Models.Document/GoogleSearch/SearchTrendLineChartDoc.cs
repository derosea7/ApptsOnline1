using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document.GoogleSearch
{
  public class SearchTrendLineChartDoc : Document
  {
    [JsonProperty("trends")]
    public List<SearchTrendsRaw> Trends { get; set; }
  }
}
