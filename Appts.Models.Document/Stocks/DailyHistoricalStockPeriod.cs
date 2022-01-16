using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Document.Stocks
{
  public class DailyHistoricalStockPeriodDocument : Document
  {
    [JsonProperty(PropertyName = "symbol")]
    public string Symbol { get; set; }

    [JsonProperty(PropertyName = "endRange")]
    public DateTime EndRange { get; set; }
    [JsonProperty(PropertyName = "length")]
    public int Length { get; set; }
    [JsonProperty(PropertyName = "dailyBars")]
    public List<DailyBarData> DailyBars { get; set; }

    public DailyHistoricalStockPeriodDocument()
    {
      EntityType = "DailyHistoricalStockPeriod";
    }
    
  }
}
