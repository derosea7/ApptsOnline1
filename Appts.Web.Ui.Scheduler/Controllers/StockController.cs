using Appts.Models.Document;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document.GoogleSearch;
using Appts.Web.Ui.Scheduler.ViewModels;
using Appts.Messaging.ServiceBus;
using Appts.Web.Ui.Scheduler.Services;
using Appts.Models.Rest.Stocks;
using Microsoft.AspNetCore.Authorization;
using Appts.Models.Rest;
using Appts.Models.Document.Stocks;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class StockController : Controller
  {
    private readonly IApiClient _apiClient;
    private readonly IBus _bus;
    private readonly IHttpContextResolverService _httpContext;
    public StockController(IApiClient apiClient, IBus bus, IHttpContextResolverService httpContext)
    {
      _apiClient = apiClient;
      _bus = bus;
      _httpContext = httpContext;
    }
    public IActionResult CompareSearchTrends(string symbol = "OKE")
    {
      ///// now get the ibkr historical price data
      var dailydocs = _apiClient.GetAsync<List<DailyHistoricalStockPeriodDocument>>(
        $"api/Data/GetDailyHistoricalStockPeriodDocuments?symbol={symbol}")
        .GetAwaiter().GetResult();

      var vm = new SearchTrendLineChartsVm();
      if (dailydocs.Count > 0)
      {
        var resonse = _apiClient.GetAsync<List<SearchTrendsRaw>>(
          $"api/Stock/GetGoogleSearchTrendGroups").GetAwaiter().GetResult();
        //            GetGoogleSearchTrendGroups
        //            GetGoogelSearchTrendGroups
        SearchTrendLineChartDoc doc = new SearchTrendLineChartDoc();
        doc.Trends = resonse;
        vm.SearchTrendsRawJson = SerializeDocument(doc);


        var prices = dailydocs.First();
        var pricelist = new List<StockComparePriceDataDoc>();
        foreach (var trend in resonse)
        {
          foreach (DateTime dateTime in trend.DateString)
          {
            foreach (var price in prices.DailyBars)
            {
              if (price.Date == dateTime)
              {
                // matching trend to stock price
                pricelist.Add(new StockComparePriceDataDoc() { Date = dateTime, Price1 = price.Close });
                break;
              }
            }
          }
        }
        JsonSerializerSettings serializer = new JsonSerializerSettings();
        serializer.NullValueHandling = NullValueHandling.Ignore;
        serializer.TypeNameHandling = TypeNameHandling.Auto; //this is key
        string pricelistjson = JsonConvert.SerializeObject(pricelist, serializer);
        vm.TrendsToPricesJson = pricelistjson;

        string dailyPriceHistory = JsonConvert.SerializeObject(dailydocs[0]);
        vm.DailyHistoricalJson = dailyPriceHistory;
      }
      else
      { 
        ///return empty vm
      }

      return View(vm);
    }

    public string SerializeDocument(IDocument document)
    {
      JsonSerializerSettings serializer = new JsonSerializerSettings();
      //serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
      serializer.NullValueHandling = NullValueHandling.Ignore;
      serializer.TypeNameHandling = TypeNameHandling.Auto; //this is key
      serializer.Formatting = Formatting.Indented;
      return JsonConvert.SerializeObject(document, serializer);
    }

    public IActionResult CompoundInterest()
    {
      return View();
    }
    //[Authorize]
    public IActionResult Positions()
    {
      return View();
    }
    public IActionResult Orders()
    {
      return View();
    }
    public IActionResult SaveOrder()
    {
      _apiClient.PostAsync<object>(new Document(), "");
      return View();
    }

    [Authorize]
    public IActionResult Transactions()
    {
      string userId = _httpContext.GetUserId();
      //load transactions
      var vm = new ViewTransactionsVm();
      var resonse = _apiClient.GetAsync<GetStockTransactionsResponse>(
        $"api/Stock/GetTransactionsApi?userId={userId}").GetAwaiter().GetResult();

      //foreach (StockTransactionDocument trans in resonse.Transactions)
      //{
      //  trans.
      //}

      vm.Transactions = resonse.Transactions;
      return View(vm);
    }
    [HttpPost]
    public IActionResult SaveTransaction([FromForm] SaveTransactionBindingModel bindingModel)
    {
      string userId = _httpContext.GetUserId();
      // create rest request model from binding model
      var request = new SaveStockTransactionRequest()
      { 
        UserId = userId,
        StockSymbol = bindingModel.StockSymbol,
         SharesTraded = bindingModel.SharesTraded,
         BuyPrice = bindingModel.BuyPrice,
         TimeBought = bindingModel.TimeBought,
         SellPrice = bindingModel.SellPrice,
         TimeSold = bindingModel.TimeSold,
         Broker = bindingModel.BrokerName
      };

      string json = JsonConvert.SerializeObject(request);
      //via api client, send data to api.scheduler
      // saved transaction into db
      string saveError = "";
      try
      {
        _apiClient.PostNoReturnAsync<SaveStockTransactionRequest>(
          request, "api/Stock/SaveTransactionApi");
      }
      catch (Exception ex)
      {
        saveError = ex.Message;
        //throw;
      }
      TempData["SaveResult"] = saveError;
      return RedirectToAction("Transactions");
    }
    public IActionResult Index()
    {
      //_bus.SendStockToWatchMessageAsync("TLSA");
      //_bus.SendStockToWatchMessageAsync("VIRX");
      var vm = new GetSavedSearchTrendsVm();
      //var trends = _apiClient.GetAsync<List<SearchTrend>>
      //  ("api/Stock/GetGoogelSearchTrends")
      //  .GetAwaiter().GetResult();
      //List<SearchTrend> t1hr = trends.Where(t => t.Timeframe == "1h").ToList();
      //List<SearchTrend> t7day = trends.Where(t => t.Timeframe == "7d").ToList();

      //List<string> sevenDaySymbols = new List<string>();

      //SearchTrend weekTrend;
      //bool hasLittleWeeklyInterest = true;
      //foreach (SearchTrend trend in t1hr)
      //{
      //  weekTrend = t7day.FirstOrDefault(t => t.StockSymbol == trend.StockSymbol);

      //  // now have 1 day and 7 day in 1 place
      //  hasLittleWeeklyInterest = true;
      //  foreach (InterestOverTime weeklyIot in weekTrend.InterestOverTime)
      //  {
      //    if (weeklyIot.Interest > (byte)SearchInterestLevel.Medium)
      //    {
      //      hasLittleWeeklyInterest = false;
      //      break;
      //    }
      //  }
      //  if (hasLittleWeeklyInterest == true)
      //  {
      //    sevenDaySymbols.Add(weekTrend.StockSymbol);
      //  }
      //}

      //vm.TrendsCount = trends.Count;
      //vm.SearchTrendsJson = JsonConvert.SerializeObject(heat);
      //string json = JsonConvert.SerializeObject(trends);
      return View(vm);
    }
    private List<SearchTrendHeatMapVm> GetHeatMap(List<SearchTrend> trends)
    {
      List<SearchTrendHeatMapVm> heat = new List<SearchTrendHeatMapVm>();
      foreach (SearchTrend trend in trends)
      {
        foreach (InterestOverTime iot in trend.InterestOverTime)
        {
          heat.Add(new SearchTrendHeatMapVm()
          {
            Symbol = trend.StockSymbol,
            Time = iot.Date,
            Interest = iot.Interest
          });
        }
      }
      return heat;
    }
    [HttpPost]
    public IActionResult PostGoogleTrends([FromBody] string json)
    {
      string j = json;
      return View();
    }
  }
}