using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Web.Ui.Scheduler.ViewModels;
using Appts.Models.Rest;
using Microsoft.ApplicationInsights;
using Appts.Models.Document;
using Appts.Models.Document.Stocks;
using Newtonsoft.Json;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  [Authorize("ApptsAdmin")]
  public class AdminController : Controller
  {
    private readonly IApiClient _apiClient;
    private TelemetryClient _telemetry;
    public AdminController(IApiClient apiClient, TelemetryClient telemetry)
    {
      _apiClient = apiClient;
      _telemetry = telemetry;
    }
    public IActionResult Index(string success = null, int? deln = null)
    {
      _telemetry.TrackPageView("Admin");
      var model = new AdminDeleteDataVm();
      if (deln != null && deln > 0)
      {
        model.DeletedCount = (int)deln;
      }
      return View(model);
    }
    public IActionResult StockDataDocsByPartitionId()
    {
      var dailydocs = _apiClient.GetAsync<List<DailyHistoricalStockPeriodDocument>>(
        "api/Data/GetDailyHistoricalStockPeriodDocuments")
        .GetAwaiter().GetResult();

      var comps = new List<StockComparePriceDataDoc>();
      var vm = new DailyHistoricalStockPeriodVm();
      vm.DailyHistoricalDocsJson = new List<string>() { };
      
      //// agg at the daily level
      bool aggset = false;
      foreach (DailyHistoricalStockPeriodDocument daily in dailydocs)
      {
        string s = daily.Symbol;

        // serialize daily into string
        // add to daily historicaldocsjson list
        vm.DailyHistoricalDocsJson.Add(SerializeDocument(daily));

        if (aggset == false)
        {
          //// initialize an empty list of compare docs
          foreach (var barData in daily.DailyBars)
          {

            comps.Add(new StockComparePriceDataDoc()
            {
              Date = barData.Date
            });
          }
        }

        if (daily.Symbol == "QQQ")
        {
          ////// dig into daily bars and add price data for price compares
          foreach (var d in comps)
          {

            foreach (var barData in daily.DailyBars)
            {
              if (d.Date == barData.Date)
              {
                d.Price1 = barData.Close;
                d.Quantity = barData.Volume;
                break;
              }
            }
          }
        }

        if (daily.Symbol == "TNX")
        {
          ////// dig into daily bars and add price data for price compares
          foreach (var d in comps)
          {

            foreach (var barData in daily.DailyBars)
            {
              if (d.Date == barData.Date)
              {
                d.Price2 = barData.Close;
                break;
              }
            }
          }
        }

        if (daily.Symbol == "OXY")
        {
          ////// dig into daily bars and add price data for price compares
          foreach (var d in comps)
          {

            foreach (var barData in daily.DailyBars)
            {
              if (d.Date == barData.Date)
              {
                d.Price3 = barData.Close;
                break;
              }
            }
          }
        }

        ///// setting this to true will enable the collection
        aggset = true;
      }

      ////create compare chart vm for amcharts chart.data
      //var compareVm = new StocksCompareChartVm();
      ////compareVm.Date = 
      var compareData = new StockCompareChartDataDoc();
      compareData.PriceDataDocs = comps;

      vm.StockComparePriceDataJson = SerializeDocument(compareData);

      //vm.StockComparePriceDataJson = SerializeDocument(Stock)

      //var jsonDocs = new List<>
      return View(vm);
    }
    public IActionResult DataByUser()
    {
      var lookupDocuments = _apiClient.GetAsync<List<LookupDocument>>(
        "api/Data/GetVanityLookups")
        .GetAwaiter().GetResult();

      var lookupVms = new List<ViewLookupDocumentVm>();
      foreach (LookupDocument lookup in lookupDocuments)
      {
        //serialize in human readable json
        //add to list
        lookupVms.Add(new ViewLookupDocumentVm()
        { 
          LookupModel = lookup,
          LookupDocumentJson = SerializeDocument(lookup)
        });
      }
      var vm = new ViewVanityUrlLookupsVm()
      { 
        LookupDocuments = lookupVms
      };
      return View(vm);
    }
    public ViewDocumentsForUserVm GetUserDocumentsById(string userId)
    {
      var vm = new ViewDocumentsForUserVm();
      var response = _apiClient.GetAsync<ViewDocumentsByUserIdResponse>(
        $"api/Data/GetUserDocs?userId={userId}").GetAwaiter().GetResult();
      if (response.Subscription != null)
      {
        vm.SubscriptionJson = SerializeDocument(response.Subscription);
      }
      if (response.ServiceProvider != null)
      {
        vm.ServiceProviderJson = SerializeDocument(response.ServiceProvider);
      }
      if (response.TrialPromo != null)
      {
        vm.TrialPromoJson = SerializeDocument(response.TrialPromo);
      }
      if (response.StripeCustomer != null)
      {
        vm.StripeCustomerJson = SerializeDocument(response.StripeCustomer);
      }
      if (response.Client != null)
      {
        vm.ClientJson = SerializeDocument(response.Client);
      }
      if (response.Availability != null)
      {
        vm.AvailabilityJson = new List<string>();
        foreach (AvailabilityPeriod period in response.Availability)
        {
          vm.AvailabilityJson.Add(SerializeDocument(period));
        }
      }
      if (response.AppointmentTypes != null)
      {
        vm.AppointmentTypesJson = new List<string>();
        foreach (AppointmentType apptType in response.AppointmentTypes)
        {
          vm.AppointmentTypesJson.Add(SerializeDocument(apptType));
        }
      }
      if (response.Appointments != null)
      {
        vm.AppointmentsJson = new List<string>();
        foreach (Appointment appt in response.Appointments)
        {
          vm.AppointmentsJson.Add(SerializeDocument(appt));
        }
      }
      return vm;
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
    [HttpPost]
    public IActionResult DeleteData([FromForm]AdminDeleteDataVm model)
    {
      _telemetry.TrackEvent("DataDeletedByAdmin");
      var s = ModelState.ValidationState;
      if (!ModelState.IsValid)
      {
        return RedirectToAction("Index", "Admin", new { invalid="t" });
      }
      var request = new BulkDeleteDataRequest() 
      { 
        UserId = model.UserId, 
        EntityType = model.EntityType, 
        DeleteAllEntities = model.DeleteAllEntities 
      };
      var countDeleted = _apiClient.PostAsync<BulkDeleteDataRequest, BulkDeleteDataResponse>(
        request, "api/Data/BulkDeleteByEntityType")
        .GetAwaiter().GetResult();
      return RedirectToAction("Index", "Admin", new { success="t", deln=countDeleted.Deleted });
    }
  }
}