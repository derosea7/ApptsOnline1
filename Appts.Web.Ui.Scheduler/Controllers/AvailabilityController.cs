using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Appts.Models.Rest;
using Appts.Models.Document;
using Appts.Common.Constants;
using Appts.Models.View;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Appts.Models.Api;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  //[Authorize]
  public class AvailabilityController : Controller
  {
    private readonly IApiClient _apiClient;
    private TelemetryClient _telemetry;
    public AvailabilityController(
      IApiClient apiClient,
      TelemetryClient telemetry)
    {
      _apiClient = apiClient;
      _telemetry = telemetry;
    }
    // GET: /<controller>/
    [Authorize("PaidSubscriber")]
    public IActionResult Index()
    {
      _telemetry.TrackPageView("AvailabilityIndex");
      string userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      var model = new ViewAvailabilityPeriodsViewModel();
      // load periods from db.
      var periods = _apiClient.GetAsync<List<AvailabilityPeriod>>(
        $"/api/Availability/GetPeriods/{userId}")
        .GetAwaiter().GetResult();
      model.Periods = periods;
      model.PeriodsJson = JsonConvert.SerializeObject(periods);
      var dates = new List<GanttRecord>();
      int i = 1;
      foreach (var period in periods)
      {
        dates.Add(new GanttRecord()
        {
          Name = "Period " + i,
          ToDate = period.EndDate?.ToString("yyyy-MM-dd"),
          FromDate = period.StartDate.ToString("yyyy-MM-dd"),
          Color = "#a36"
        });
        i++;
      }
      model.PeriodDatesJson = JsonConvert.SerializeObject(dates);
      return View(model);
    }
    /// <summary>
    /// Return model that can be used when adding or editing an availability period.
    /// </summary>
    /// <returns></returns>
    private AddAvailabilityPeriodViewModel GetInitialModel()
    {
      string userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      var model = new AddAvailabilityPeriodViewModel()
      {
        PeriodDates = new List<PeriodDatesApiModel>()
      };
      var response = _apiClient.GetAsync<GetPeriodDatesResponse>($"/api/Availability/GetPeriodDates/{userId}")
        .GetAwaiter().GetResult();
      foreach (var item in response.PeriodDates)
      {
        model.PeriodDates.Add(new PeriodDatesApiModel()
        {
          PeriodStart = item.PeriodStart,
          PeriodEnd = item.PeriodEnd
        });
      }
      model.PeriodDatesJson = JsonConvert.SerializeObject(response.PeriodDates);
      return model;
    }
    private AddAvailabilityPeriodViewModel GetInitialModelToEdit(string periodId)
    {
      string userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      var model = new AddAvailabilityPeriodViewModel()
      {
        PeriodDates = new List<PeriodDatesApiModel>()
      };
      var response = _apiClient.GetAsync<GetPeriodDatesResponse>(
        $"/api/Availability/GetPeriodDatesWithExclusion/{userId}?excludePeriodId={periodId}")
        .GetAwaiter().GetResult();
      foreach (var item in response.PeriodDates)
      {
        model.PeriodDates.Add(new PeriodDatesApiModel()
        {
          PeriodStart = item.PeriodStart,
          PeriodEnd = item.PeriodEnd
        });
      }
      model.PeriodDatesJson = JsonConvert.SerializeObject(response.PeriodDates);
      return model;
    }
    /// <summary>
    /// Get start and end dates of all periods on file for user id.
    /// </summary>
    /// <param name="userId">Service Provider to get availability periods for</param>
    /// <returns>JSON of period dates</returns>
    private string GetPeriodDates(string userId)
    {
      var periodDates = new List<PeriodDatesApiModel>();
      var response = _apiClient.GetAsync<GetPeriodDatesResponse>($"/api/Availability/GetPeriodDates/{userId}")
        .GetAwaiter().GetResult();
      foreach (var item in response.PeriodDates)
      {
        periodDates.Add(new PeriodDatesApiModel()
        {
          PeriodStart = item.PeriodStart,
          PeriodEnd = item.PeriodEnd
        });
      }
      return JsonConvert.SerializeObject(response.PeriodDates);
    }
    [Authorize("PaidSubscriber")]
    public IActionResult Add()
    {
      _telemetry.TrackPageView("AddAvailaiblity");
      var model = GetInitialModel();
      return View(model);
    }
    private void ValidateAddAvailabilityModel(AddAvailabilityPeriodViewModel model)
    {
      bool hasAtLeastOneBlock = false;
      foreach (Availability day in model.AvailabilityDays)
      {
        Dictionary<string, string> days = GetValidAvailabilityDays();
        if (!days.Any(d => d.Key == day.DayOfWeek))
        {
          ModelState.AddModelError("DayOfWeek", "One or more days of availability does not have a valid day of week");
        }
        if (day.Blocks != null)
        {
          hasAtLeastOneBlock = true;
          foreach (AvailabilityBlock block in day.Blocks)
          {
            // TODO: add validation
            // replicate front-end validation here
            // overlap beginning, end
            // encompasses, encompassed
          }
        }
      } // for each day
      if (!hasAtLeastOneBlock)
      {
        ModelState.AddModelError("MustHaveAtLeastOneAvailableBlock", "Must have at least one day of availability");
      }
    }
    [HttpPost]
    public IActionResult Add(AddAvailabilityPeriodViewModel model)
    {
      _telemetry.TrackEvent("AddAvailabilityRequested");
      ValidateAddAvailabilityModel(model);
      if (!ModelState.IsValid)
      {
        // TODO: append to existing model instead of wiping it out
        //model = GetInitialModel();
        var initialModel = GetInitialModel();
        model.PeriodDates = initialModel.PeriodDates;
        model.PeriodDatesJson = initialModel.PeriodDatesJson;
        model.AvailabilityDaysJson = JsonConvert.SerializeObject(model.AvailabilityDays);
        return View(model);
      }
      string userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      var request = new SavePeriodRequest()
      {
        ServiceProviderId = userId,
        PeriodStart = model.PeriodStart,
        PeriodEnd = model.PeriodEnd,
        AvailabilityDays = model.AvailabilityDays
      };
      _apiClient.PostNoReturnAsync<SavePeriodRequest>(
        request, $"/api/Availability/SavePeriod")
        .GetAwaiter().GetResult();
      return RedirectToAction("Index", new { c = "t" });
    }
    [Authorize("PaidSubscriber")]
    [HttpGet("[controller]/[action]/{id}")]
    public IActionResult Edit(string id)
    {
      _telemetry.TrackPageView("UpdateAvailability");
      // will require slightly different model for editing scenario
      // when loading period dates, will want to exclude the current period
      // so that the sp could use the dates if they edit start or end
      //var model = GetInitialModel();
      var model = GetInitialModelToEdit(id);
      model.IsEditScenario = true;
      string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      // get details of period of id
      string endpoint = $@"/api/availability/GetPeriod?userId={userId}&periodId={id}";
      var period = _apiClient.GetAsync<AvailabilityPeriod>(endpoint).GetAwaiter().GetResult();
      //model.AvailabilityDays = period.Availability;
      model.AvailabilityDaysJson = JsonConvert.SerializeObject(period.Availability);
      model.PeriodJson = JsonConvert.SerializeObject(period);
      model.PeriodStart = period.StartDate;
      model.PeriodEnd = period.EndDate;
      model.PeriodId = period.Id;
      return View("Add", model);
    }
    [Authorize("PaidSubscriber")]
    [ActionName("Edit")]
    [HttpPost("[controller]/[action]/{id}")]
    public IActionResult EditPost(string id, AddAvailabilityPeriodViewModel model)
    {
      _telemetry.TrackEvent("UpdateAvailabilityRequested");
      string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      string endpoint = $@"/api/availability/updateperiod";
      var request = new UpdatePeriodRequest()
      {
        AvailabilityDays = model.AvailabilityDays,
        PeriodStart = model.PeriodStart,
        PeriodEnd = model.PeriodEnd,
        ServiceProviderId = userId,
        PeriodId = model.PeriodId
      };
      _apiClient.PatchNoReturnAsync<UpdatePeriodRequest>(request, endpoint)
        .GetAwaiter().GetResult();
      // send model to api to replace in db
      return RedirectToAction("Index", new { e = "t" });
    }
    [HttpGet]
    public GetAvailabilityForSchedulingResponse GetAvailabilityForScheduling(
      string serviceProviderId, string apptTypeId, DateTime startRangeDate, bool oneWeekOnly)
    {
      string endpoint = $@"/api/availability/GetAvailabilityForScheduling?"
        + $"ServiceProviderId={serviceProviderId}"
        + $"&AppointmentTypeId={apptTypeId}"
        + $"&StartDateRange={startRangeDate}"
        + $"&OneWeekOnly={oneWeekOnly}";
      return _apiClient.GetAsync<GetAvailabilityForSchedulingResponse>(endpoint)
        .GetAwaiter().GetResult();
    }
    [HttpPost]
    //public bool SaveBlocks([FromBody] List<AvailabilityBlock> blocks)
    public JsonResult SaveBlocks([FromBody]SaveTimeBlocksRequest model)
    {
      bool s = ModelState.IsValid;
      foreach (AvailabilityBlock block in model.Blocks)
      {
        if (block.EndTime <= block.StartTime)
        {
          ModelState.AddModelError("EndGreaterThanStart", "End time cannot be greter than start time");
        }
      }
      if (!ModelState.IsValid)
      {
        //return Json(new { Ok = false });
        Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //return false;
        return Json(new { responseText = "Invalid blocks" });
      }
      return Json("Ok");
    }
    [HttpPost("[controller]/[action]/{periodId}")]
    public IActionResult DeletePeriod(string periodId)
    {
      _telemetry.TrackEvent("DeletePeriodRequested");
      string userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      var request = new DeleteAvailabilityPeriodRequest()
      {
        PeriodId = periodId,
        UserId = userId
      };
      _apiClient.PostNoReturnAsync<DeleteAvailabilityPeriodRequest>(
        request, $"/api/Availability/DeletePeriod").GetAwaiter().GetResult();
      return RedirectToAction("Index", new { d = "t" });
    }
    private Dictionary<string, string> GetValidAvailabilityDays()
    {
      return new Dictionary<string, string>()
      {
        { "Sunday", "Sun" },
        { "Monday", "Mon" },
        { "Tuesday", "Tue" },
        { "Wednesday", "Wed" },
        { "Thursday", "Thu" },
        { "Friday", "Fri" },
        { "Saturday", "Sat" }
      };
    }
  }
}