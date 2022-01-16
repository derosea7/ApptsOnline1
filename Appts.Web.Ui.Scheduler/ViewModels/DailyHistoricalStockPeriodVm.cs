using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class DailyHistoricalStockPeriodVm
  {
    public List<string> DailyHistoricalDocsJson { get; set; }
    public string StockComparePriceDataJson { get; set; }
  }
}
