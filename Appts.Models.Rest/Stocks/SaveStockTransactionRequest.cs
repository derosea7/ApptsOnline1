using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest.Stocks
{
  public class SaveStockTransactionRequest
  {
    public string UserId { get; set; }
    public string StockSymbol { get; set; }
    public double SharesTraded { get; set; }
    
    //buying
    public double BuyPrice { get; set; }
    public DateTime TimeBought { get; set; }

    //selling
    public double SellPrice { get; set; }
    public DateTime TimeSold { get; set; }

    public string Broker { get; set; }
  }
}
