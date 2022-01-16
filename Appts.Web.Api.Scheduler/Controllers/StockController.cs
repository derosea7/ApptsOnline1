using Appts.Dal.Cosmos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document.GoogleSearch;
using Appts.Models.Document.Stocks;
using Appts.Models.Rest.Stocks;
using Appts.Models.Rest;
namespace Appts.Web.Api.Scheduler.Controllers
{
  public class StockController : Controller
  {
    private readonly IDb _db;
    public StockController(IDb db)
    {
      _db = db;
    }
    #region "Search Trends"
    //public List<SearchTrendsRaw> GetGoogelSearchTrendGroups(DateTime utcSearchEnded)
    public List<SearchTrendsRaw> GetGoogleSearchTrendGroups()
    {
      return _db.GetMultipeAsync<SearchTrendsRaw>
        //($"select * from c where c.entityType = 'SearchTrendGroupRaw' and c.userId = 'SearchTrendGroupRaw_{utcSearchEnded.ToString("yyyy-MM-dd")}' ")
        ($"select * from c where c.entityType = 'SearchTrendGroupRaw' and c.userId = 'SearchTrendGroupRaw' ")
        .GetAwaiter().GetResult();
    }
    [HttpPost]
    public void PostGoogleTrendsApi([FromQuery] string timeframe, [FromBody] SearchTrendsRaw json)
    {
      //var trends = CreateSearchTrend(json, timeframe);
      //foreach (SearchTrend trend in trends)
      //{
      //  _db.CreateNoReturnAsync(trend).GetAwaiter().GetResult();
      //}
      _db.CreateNoReturnAsync(json);
      //_db.
    }
    private List<SearchTrend> CreateSearchTrend(SearchTrendsRaw json, string timeframe)
    {
      int i = 0;
      List<SearchTrend> trends = new List<SearchTrend>();
      //SearchTrend t = new SearchTrend();
      //foreach (string symbol in json.Columns)
      //{
      //  int symbseqno = 1;
      //  t = new SearchTrend()
      //  {
      //    StockSymbol = symbol,
      //    Timeframe = timeframe,
      //    InterestOverTime = new List<InterestOverTime>()
      //  };
      //  foreach (byte[] item in json.Interest)
      //  {
      //    byte b = item[symbseqno];
      //    var seconds = json.DateAsSecondsUtc[i];
      //    var iot = new InterestOverTime();
      //    iot.Date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
      //    iot.Date = iot.Date.AddMilliseconds(seconds);
      //    iot.Interest = b;
      //    t.InterestOverTime.Add(iot);
      //    i++;
      //  }
      //  t.Endtime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddMilliseconds(json.DateAsSecondsUtc[i - 1]);
      //  trends.Add(t);
      //  symbseqno += 1;
      //}

      //var trends = new SearchTrend()
      //{
      //  StockSymbol = json.Columns[0],
      //  Timeframe = timeframe,
      //  InterestOverTime = new List<InterestOverTime>()
      //};

      return trends;
    }
    #endregion
    #region "Stock Transactions"
    [HttpPost]
    public IActionResult SaveTransactionApi([FromBody] SaveStockTransactionRequest request)
    {
      StockTransactionDocument doc = new StockTransactionDocument()
      { 
        StockSymbol = request.StockSymbol,
        SharesTraded = request.SharesTraded,
        BuyPricePerShare = request.BuyPrice,
        SellPricePerShare = request.SellPrice,
        Id = Guid.NewGuid().ToString(),
        UserId = request.UserId,
        TimeBought = request.TimeBought,
        TimeSold = request.TimeSold,
        Borker = request.Broker
      };
      doc.BuyTotal = doc.BuyPricePerShare * doc.SharesTraded;
      doc.SellTotal = doc.SellPricePerShare * doc.SharesTraded;
      doc.RoiPercent = doc.SellPricePerShare / doc.BuyPricePerShare;
      doc.RoiTotal = (doc.SellPricePerShare * doc.SharesTraded) - (doc.BuyPricePerShare * doc.SharesTraded);
      doc.RoiAt10k = (10000 * doc.RoiPercent) - 10000;
      _db.CreateNoReturnAsync(doc).GetAwaiter().GetResult();
      return View();
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
    #endregion
  }
}