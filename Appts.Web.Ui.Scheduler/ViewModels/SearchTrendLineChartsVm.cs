using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class SearchTrendLineChartsVm
  {
    public string SearchTrendsRawJson { get; set; }

    public string TrendsToPricesJson { get; set; }
    public string DailyHistoricalJson { get; set; }
  }
}
