using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Dal.Cosmos;
using Appts.Models.Document.GoogleSearch;
using Appts.Models.Document.Stocks;
using Appts.Models.Rest.Stocks;
using Appts.Models.Rest;
namespace Appts.Web.Api.Scheduler.Controllers
{
  public class PositionController : Controller
  {
    private readonly IDb _db;
    public PositionController(IDb db)
    {
      _db = db;
    }
    public GetStockTransactionsResponse GetTransactionsApi(string userId)
    {
      return new GetStockTransactionsResponse()
      {
        Transactions = _db.GetMultipeAsync<StockTransactionDocument>(
          $"select * from c where c.userId = '{userId}' and c.entityType = 'StockTrans' ")
          .GetAwaiter().GetResult()
      };
    }
  }
}
