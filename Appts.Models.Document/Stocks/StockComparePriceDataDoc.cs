using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document.Stocks
{
  public class StockComparePriceDataDoc : Document
  {
    [JsonProperty(PropertyName = "date")]
    public DateTime Date { get; set; }

    [JsonProperty(PropertyName = "price1")]
    public decimal Price1 { get; set; }
    [JsonProperty(PropertyName = "price2")]
    public decimal Price2 { get; set; }
    [JsonProperty(PropertyName = "price3")]
    public decimal Price3 { get; set; }
    [JsonProperty(PropertyName = "quantity")]
    public decimal Quantity { get; set; }
  }
}
