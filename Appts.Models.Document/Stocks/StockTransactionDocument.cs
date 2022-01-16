using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document.Stocks
{
  public class StockTransactionDocument : Document
  {
    public StockTransactionDocument()
    {
      EntityType = "StockTrans";
    }
    [JsonProperty("symbol")]
    public string StockSymbol { get; set; }
    [JsonProperty("sharestraded")]
    public double SharesTraded { get; set; }
    [JsonProperty("buyprice")]
    public double BuyPricePerShare { get; set; }
    [JsonProperty("timebought")]
    public DateTime TimeBought { get; set; }
    [JsonProperty("sellprice")]
    public double SellPricePerShare { get; set; }
    [JsonProperty("timesold")]
    public DateTime TimeSold { get; set; }
    [JsonProperty("broker")]
    public string Borker { get; set; }

    //calculated columns
    [JsonProperty("buytotal")]
    public double BuyTotal { get; set; }
    [JsonProperty("selltotal")]
    public double SellTotal { get; set; }
    [JsonProperty("roiperc")]
    public double RoiPercent { get; set; }
    [JsonProperty("roitot")]
    public double RoiTotal { get; set; }
    [JsonProperty("roiat10k")]
    public double RoiAt10k { get; set; }

  }
}
