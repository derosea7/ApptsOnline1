using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Appts.Models.Document.Stocks
{
  public class DailyBarData
  {
    [JsonProperty(PropertyName = "date")]
    public DateTime Date { get; set; }

    [JsonProperty(PropertyName = "open")]
    public decimal Open { get; set; }
    [JsonProperty(PropertyName = "high")]
    public decimal High { get; set; }
    [JsonProperty(PropertyName = "low")]
    public decimal Low { get; set; }
    [JsonProperty(PropertyName = "close")]
    public decimal Close { get; set; }

    [JsonProperty(PropertyName = "volume")]
    public decimal Volume { get; set; }

    public override string ToString()
    {
      return $"date: {Date}, o {Open}, h {High}, l {Low}, c: {Close}, v: {Volume}";
    }
  }
}
