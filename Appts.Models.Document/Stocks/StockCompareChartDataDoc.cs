using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Document.Stocks
{
  public class StockCompareChartDataDoc : Document
  {
    //[JsonProperty(PropertyName = "date")]
    //public DateTime Date { get; set; }

    [JsonProperty(PropertyName = "PriceDataDocs")]
    public List<StockComparePriceDataDoc> PriceDataDocs { get; set; }

  }
}
