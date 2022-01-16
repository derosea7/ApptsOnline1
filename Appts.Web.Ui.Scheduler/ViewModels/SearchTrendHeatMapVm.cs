using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class SearchTrendHeatMapVm
  {
    [JsonProperty("symbol")]
    public string Symbol { get; set; }
    [JsonProperty("time")]
    public DateTime Time { get; set; }
    [JsonProperty("interest")]
    public int Interest { get; set; }
    [JsonProperty("timeframe")]
    public string Timeframe { get; set; }
  }
}
