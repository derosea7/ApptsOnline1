using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document.GoogleSearch;
namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class GetSavedSearchTrendsVm
  {
    public List<SearchTrend> SearchTrends { get; set; }
    public string SearchTrendsJson { get; set; }
    public int TrendsCount { get; set; }
  }
}
