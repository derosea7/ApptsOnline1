using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class SaveTransactionBindingModel
  {
    public DateTime RecordedTime { get; set; }
    public string StockSymbol { get; set; }
    public double SharesTraded { get; set; }
    public double BuyPrice { get; set; }
    public DateTime TimeBought { get; set; }
    public double SellPrice { get; set; }
    public DateTime TimeSold { get; set; }

    public string BrokerName { get; set; }
  }
}
