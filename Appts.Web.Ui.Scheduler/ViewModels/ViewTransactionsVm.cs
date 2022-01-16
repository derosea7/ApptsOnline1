using Appts.Models.Document.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class ViewTransactionsVm
  {
    public List<StockTransactionDocument> Transactions { get; set; }

    // binding model on the save
    public SaveTransactionBindingModel BindingModel { get; set; }
  }
}
