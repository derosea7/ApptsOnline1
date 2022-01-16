using Appts.Models.Document.Stocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class GetStockTransactionsResponse
  {
    public List<StockTransactionDocument> Transactions { get; set; }
  }
}
