using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Common.Constants;
using Appts.Models.Document;
using Appts.Models.View;
using Microsoft.AspNetCore.Mvc;
using Appts.Models.Rest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class AppointmentTypeController : Controller
  {
    private readonly IApiClient _apiClient;
    private TelemetryClient _telemetry;
    public AppointmentTypeController(IApiClient apiClient, TelemetryClient telemetry)
    {
      _apiClient = apiClient;
      _telemetry = telemetry;
    }
    [Authorize("PaidSubscriber")]
    public IActionResult Index()
    {
      _telemetry.TrackPageView("ApptTypeIndex");
      string userId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      List<AppointmentType> types = _apiClient.GetAsync<List<AppointmentType>>(
        $"/api/appointmenttype/GetAppointmentTypes?userId={userId}")
        .GetAwaiter().GetResult();
      //TODO: filter this sooner upstream; temp for dev & IR1
      //List<AppointmentType> activeTypes = types
      //  .Where(at => at.Deleted == false)
      //  .ToList();
      return View(types);
    }
    [Authorize("PaidSubscriber")]
    [HttpPost("[controller]/[action]/{appointmentTypeId}")]
    public IActionResult Delete(string appointmentTypeId)
    {
      _telemetry.TrackEvent("ApptTypeDeletionRequested");
      string userId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      var request = new DeleteAppointmentTypeRequest()
      {
        AppointmentTypeId = appointmentTypeId,
        ServiceProviderId = userId
      };
      _apiClient.PostNoReturnAsync<DeleteAppointmentTypeRequest>(
        request, $"/api/AppointmentType/Delete");

      return RedirectToAction("Index", new { d = "t" });
    }
    //TODO: consider how this ajax call would work if user is signed out?
    //would auth tag force browser to re-login?
    [Authorize("PaidSubscriber")]
    [Route("[controller]/[action]/{id}")]
    public AppointmentType Detail(string id)
    {
      _telemetry.TrackPageView("ApptTypeDetail");
      string userId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      var apptType = _apiClient.GetAsync<AppointmentType>(
        $"/api/AppointmentType/Detail2/{id}?userId={userId}")
        .GetAwaiter().GetResult();
      return apptType;
    }
    [Authorize("PaidSubscriber")]
    [Route("[controller]/[action]/{apptTypeId}")]
    public IActionResult Edit(string apptTypeId)
    {
      _telemetry.TrackPageView("ApptTypeAdd");
      string userId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      var apptType = _apiClient.GetAsync<AppointmentType>(
        $"/api/AppointmentType/Detail2/{apptTypeId}?userId={userId}")
        .GetAwaiter().GetResult();
      var model = new AddAppointmentTypeViewModel()
      {
        IsEditScenario = true,
        Name = apptType.Name,
        IsActive = apptType.IsActive,
        Description = apptType.Description,
        DurationHours = apptType.Duration.Hours,
        DurationMinutes = apptType.Duration.Minutes,
        Location = apptType.Location,
        CancelationNoticeDays =apptType.CancelationNotice.Days,
        CancelationNoticeHours = apptType.CancelationNotice.Hours,
        CancelationNoticeMinutes = apptType.CancelationNotice.Minutes,
        RescheduleNoticeDays = apptType.RescheduleNotice.Days,
        RescheduleNoticeHours = apptType.RescheduleNotice.Hours,
        RescheduleNoticeMinutes = apptType.RescheduleNotice.Minutes,
        MaximumNoticeDays = apptType.MaximumNotice.Days,
        MaximumNoticeHours = apptType.MaximumNotice.Hours,
        MaximumNoticeMinutes = apptType.MaximumNotice.Minutes,
        MinimumNoticeDays = apptType.MinimumNotice.Days,
        MinimumNoticeHours = apptType.MinimumNotice.Hours,
        MinimumNoticeMinutes = apptType.MinimumNotice.Minutes,
        BufferBeforeHours = apptType.BufferBefore.Hours,
        BufferBeforeMinutes = apptType.BufferBefore.Minutes,
        BufferAfterHours = apptType.BufferAfter.Hours,
        BufferAfterMinutes = apptType.BufferAfter.Minutes,
      };
      switch (apptType.Location)
      {
        case "we_specify":
          model.LocationDetails_WeSpecify = apptType.LocationDetails;
          break;
        case "web_conference":
          model.LocationDetails_WebConf = apptType.LocationDetails;
          break;
        case "customer_calls":
          model.LocationDetails_CustomerCalls = apptType.LocationDetails;
          break;
        default:
          break;
      }
      return View("Add", model);
    }
    [Authorize("PaidSubscriber")]
    [HttpPost]
    [Route("[controller]/[action]/{apptTypeId}")]
    public IActionResult Edit(string apptTypeId, AddAppointmentTypeViewModel model)
    {
      _telemetry.TrackEvent("ApptTypeEditRequested");
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var apptType = new AppointmentType();
      apptType.Name = model.Name;
      apptType.IsActive = model.IsActive;
      apptType.Duration = new TimeSpan(
        (model.DurationHours ?? 0),
        (model.DurationMinutes ?? 0),
        0);
      apptType.Description = model.Description;
      //location
      apptType.Location = model.Location;
      switch (apptType.Location)
      {
        case "web_conference":
          apptType.LocationDetails = model.LocationDetails_WebConf;
          break;
        case "customer_calls":
          apptType.LocationDetails = model.LocationDetails_CustomerCalls;
          break;
        case "we_specify":
          apptType.LocationDetails = model.LocationDetails_WeSpecify;
          break;
        default:
          break;
      }
      MapTimeSpans(apptType, model);
      string userId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      apptType.UserId = userId;
      apptType.Id = apptTypeId;
      _apiClient.PatchNoReturnAsync<AppointmentType>(apptType,
        $"/api/AppointmentType/Update")
        .GetAwaiter().GetResult();
      return RedirectToAction("Index", new { u = "t" });
    }
    [Authorize("PaidSubscriber")]
    public IActionResult Add()
    {
      _telemetry.TrackPageView("ApptTypeAdd");
      return View();
    }
    [Authorize("PaidSubscriber")]
    [HttpPost]
    public IActionResult Add(AddAppointmentTypeViewModel model)
    {
      _telemetry.TrackEvent("ApptTypeAddRequested");
      if (!ModelState.IsValid)
      {
        return View(model);
      }
      var apptType = new AppointmentType();
      apptType.Name = model.Name;
      apptType.IsActive = model.IsActive;
      apptType.Duration = new TimeSpan(
        (model.DurationHours ?? 0),
        (model.DurationMinutes ?? 0),
        0);
      apptType.Description = model.Description;
      apptType.Location = model.Location;
      switch (apptType.Location)
      {
        case "web_conference":
          apptType.LocationDetails = model.LocationDetails_WebConf;
          break;
        case "customer_calls":
          apptType.LocationDetails = model.LocationDetails_CustomerCalls;
          break;
        case "we_specify":
          apptType.LocationDetails = model.LocationDetails_WeSpecify;
          break;
        default:
          break;
      }
      MapTimeSpans(apptType, model);
      string userId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      apptType.UserId = userId;
      apptType.Id = Guid.NewGuid().ToString();
      _apiClient.PostNoReturnAsync<AppointmentType>(apptType,
        $"/api/AppointmentType/Create")
        .GetAwaiter().GetResult();
      return RedirectToAction("Index", new { c = "t" });
    }
    private void MapTimeSpans(AppointmentType apptType, AddAppointmentTypeViewModel model)
    {
      apptType.CancelationNotice = new TimeSpan(
      (model.CancelationNoticeDays ?? 0),
      (model.CancelationNoticeHours ?? 0),
      (model.CancelationNoticeMinutes ?? 0),
      0);
      apptType.RescheduleNotice = new TimeSpan(
        (model.RescheduleNoticeDays ?? 0),
        (model.RescheduleNoticeHours ?? 0),
        (model.RescheduleNoticeMinutes ?? 0),
        0);
      apptType.MinimumNotice = new TimeSpan(
        (model.MinimumNoticeDays ?? 0),
        (model.MinimumNoticeHours ?? 0),
        (model.MinimumNoticeMinutes ?? 0),
        0);
      apptType.MaximumNotice = new TimeSpan(
        (model.MaximumNoticeDays ?? 0),
        (model.MaximumNoticeHours ?? 0),
        (model.MaximumNoticeMinutes ?? 0),
        0);

      apptType.BufferBefore = new TimeSpan(
        (model.BufferBeforeHours ?? 0),
        (model.BufferBeforeMinutes ?? 0),
        0);

      apptType.BufferAfter = new TimeSpan(
        (model.BufferAfterHours ?? 0),
        (model.BufferAfterMinutes ?? 0),
        0);
    }
  }
}
