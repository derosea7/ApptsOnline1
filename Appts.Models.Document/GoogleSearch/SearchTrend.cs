using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document.GoogleSearch
{
  public class SearchTrend : Document
  {
    public SearchTrend()
    {
      EntityType = "SearchTrend";
      //todo:search trend
      UserId = "SearchTrend";
      Id = Guid.NewGuid().ToString();
    }
    [JsonProperty("symb")]
    public string StockSymbol { get; set; }

    //1hr, 4hrs, 1day, 7days
    [JsonProperty("timeframe")]
    public string Timeframe { get; set; }

    [JsonProperty("endtime")]
    public DateTime Endtime { get; set; }

    [JsonProperty("iovert")]
    public List<InterestOverTime> InterestOverTime { get; set; }
    public override string ToString()
    {
      return $"{StockSymbol} {Timeframe} {Endtime.ToString()}";
    }
  }
}
