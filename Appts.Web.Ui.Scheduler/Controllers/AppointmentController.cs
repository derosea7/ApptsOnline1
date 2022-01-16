using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Web.Ui.Scheduler.Repositories;
using Appts.Common.Constants;
using Appts.Models.Document;
using Appts.Models.View;
using Microsoft.AspNetCore.Mvc.Rendering;
using Appts.Models.Domain;
using Appts.Models.Rest;
using Appts.Web.Ui.Scheduler.ViewModels;
using Appts.Web.Ui.Scheduler.Models;
using Appts.Web.Ui.Scheduler.Services;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  /// <summary>
  /// Used to retrieve appointments for the user.
  /// </summary>
  //[Authorize]
  public class AppointmentController : Controller
  {
    private readonly IAppointmentRepository _apptRepository;
    private readonly IHttpContextResolverService _httpContext;
    private readonly IConfiguration _config;
    private TelemetryClient _telemetry;
    private readonly IApptTelemetryRepository _appttel;
    public AppointmentController(
      IAppointmentRepository apptRepository,
      IHttpContextResolverService httpContextResolverService,
      IConfiguration config,
      TelemetryClient telemetry,
      IApptTelemetryRepository appttel)
    {
      _apptRepository = apptRepository;
      _httpContext = httpContextResolverService;
      _config = config;
      _telemetry = telemetry;
      _appttel = appttel;
    }
    // post prevents timezone from being saved in querysting
    [HttpPost]
    public IActionResult RefreshTimes(
      [FromBody] UpdateTimeZoneViewModel newTimeZone,
      DateTime? start = null, DateTime? end = null,
      string AppointmentTypeId = null, string AppointmentStatus = null)
    {
      _telemetry.TrackEvent("RefreshAppointmentTimes");
      return RedirectToAction("Index2", new
      {
        start = start,
        end = end,
        AppointmentTypeId = AppointmentTypeId,
        AppointmentStatus = AppointmentStatus,
        newTimeZone.TimeZoneId
      });
    }
    [Authorize]
    public IActionResult Index(
      DateTime? start = null, DateTime? end = null,
      string AppointmentTypeId = null, string AppointmentStatus = null,
      string tz = null)
    {
      _telemetry.TrackPageView("AppointmentsIndexRequested");
      var model = new AppointmentsViewModel();
      string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      UserRole userRole = _httpContext.GetUserRole(userId);
      if (userRole == UserRole.Subscriber)
      {
        _telemetry.TrackEvent("ViewSpAppointmentsList");
        // show subscriber's appts
        string spid = userId; //only for conceptual purposes;
        model = GetServiceProvidersAppointmentsVm(start, end, userRole, AppointmentTypeId, AppointmentStatus, tz, spid);
      }
      else
      {
        // show client's appts
        _telemetry.TrackEvent("ViewAppointmentsListAsClient");
      }
      return View(model);
    }
    AppointmentsViewModel GetServiceProvidersAppointmentsVm(
      DateTime? start, DateTime? end, 
      UserRole userRole,
      string AppointmentTypeId, string AppointmentStatus, string tz,
      string userId = null)
    {
      if (userId == null)
        userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      var request = new ViewAppointmentsRequest()
      {
        UserId = userId,
        Start = (DateTime)((start == null) ? DateTime.Today.AddDays(-3) : start),
        End = (DateTime)((end == null) ? DateTime.Today.AddDays(7) : end),
        ApptTypeId = AppointmentTypeId,
        ApptStatus = AppointmentStatus,
        IanaTimeZoneId = tz,
        UserRole = userRole
      };
      ViewAppointmentsResponse response = _apptRepository.GetViewAppointmentsAsync(request)
        .GetAwaiter().GetResult();

      // iterate thru appts and mark read reciept based on user role if needed
      //if (userRole == UserRole.Subscriber)
      //{ 

      //}
      //else if (userRole == UserRole.Client)
      //{

      //}
      //format appt created and / or canceled dates
      string timezoneToUse = tz == null ? response.IanaTimeZoneId : tz;
      //CalculateTimeAgo(response.Appointments, response.IanaTimeZoneId);
      CalculateTimeAgo(response.Appointments, timezoneToUse);
      // construct the view model
      var apptsByDate = new List<AppointmentsByDateViewModel>();
      apptsByDate = SortAppointmentsByDate(response.Appointments);
      var statusOptions = new List<SelectListItem>()
      {
        new SelectListItem() { Value = "", Text = "Choose..." }
      };
      List<string> statusEnums = Enum.GetNames(typeof(EntityStatus)).ToList();
      foreach (string status in statusEnums)
      {
        statusOptions.Add(new SelectListItem() { Value = status, Text = status });
      }
      string displayTimeZoneId = (tz == null) ? response.IanaTimeZoneId : tz;
      var model = new AppointmentsViewModel()
      {
        AppointmentByDate = apptsByDate,
        AppointmentTypes = response.AppointmentTypes.ToSelectList(),
        StartDate = request.Start,
        EndDate = request.End,
        AppointmentStatusOptions = statusOptions,
        DisplayIanaTimeZoneId = displayTimeZoneId,
        SpVanityUrl = response.SpVanityUrl,
        NewApptCount = response.NewAppointments,
        NewApptIds = response.NewApptIds
      };
      return model;
    }
    private void CalculateTimeAgo(List<Appointment> appointments, string timeZone)
    {
      DateTime timeZonedNow = DateTime.MinValue;
      foreach (Appointment appointment in appointments)
      {
        if (appointment.Created.On != null)
        {
          timeZonedNow = Chronotope.CreateZonedTime(timeZone);
          appointment.Created.OnAsTimeAgo = TimeAgo.Calculate((DateTime)appointment.Created.On, timeZonedNow);
        }
      }
    }
    private List<AppointmentsByDateViewModel> SortAppointmentsByDate(List<Appointment> appointments)
    {
      var apptsByDate = new List<AppointmentsByDateViewModel>();
      if (appointments.Count > 0)
      {
        List<DateTime> distintApptDates = appointments
          //.Select(a => a.StartTime.Date)
          .Select(a => a.StartTime.Date)
          .Distinct()
          //.OrderBy(ad => ad.Date)
          //.OrderBy(ad => ad.Date + ad.TimeOfDay)
          .ToList();
        List<Appointment> daysAppointments = new List<Appointment>();
        foreach (DateTime date in distintApptDates)
        {
          //daysAppointments = appointments.Where(a => a.StartTime.Date == date).ToList();
          daysAppointments = appointments.Where(a => a.StartTime.Date == date)
            .OrderBy(ad => ad.StartTime).ToList();
          //now sort appts by time
          //foreach (Appointment appointment in daysAppointments)
          //{

          //}
          //List<Appointment> sortedAppts = daysAppointments
          //  .Select(a => a.StartTime)
          apptsByDate.Add(new AppointmentsByDateViewModel()
          {
            AppointmentDate = date.Date,
            Appointments = daysAppointments
          });
        }
      }
      return apptsByDate;
    }
    [HttpGet("[controller]/[action]/{spVanityUrl}")]
    //make appt take take spvanityurl so annon user can use
    public Appointment Detail(string spVanityUrl, string apptId)
    {
      _telemetry.TrackEvent("ViewApptDetail");
      //string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      Appointment appt = _apptRepository.GetAppointmentAsync(spVanityUrl, apptId)
        .GetAwaiter().GetResult();
      return appt;
    }
    [Authorize("SchedulingPrivacy")]
    [HttpGet("[controller]/[action]/{serviceProviderVanityUrl}")]
    public IActionResult Cancel(string serviceProviderVanityUrl, string apptId)
    {
      _telemetry.TrackEvent("InitiateCancelAppt");
      _telemetry.TrackPageView("CancelAppt");
      //BUG: fix; this is being used as spid downstream
      //so, this will only work if the current user id is a service provider.
      //fix is to use the sp's vanity url in addition to appt id.
      // then in api, lookup on vanity url can find spid and then appt can actually be found.
      ///string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      var request = new GetAppointmentDetailRequest()
      {
        //UserId = userId,
        SpVanityUrl = serviceProviderVanityUrl,
        ApptId = apptId
      };
      GetAppointmentDetailResponse response = _apptRepository.GetAppointmentToCancel(
        request).GetAwaiter().GetResult();
      var model = new CancelAppointmentViewModel()
      {
        Appointment = response.Appointment,
        IanaTimeZoneId = response.IanaTimeZoneId
      };
      return View(model);
    }
    [Authorize("SchedulingPrivacy")]
    [HttpPost("[controller]/[action]/{serviceProviderVanityUrl}")]
    [ActionName("Cancel")]
    public IActionResult CancelPatch(string serviceProviderVanityUrl, CancelAppointmentViewModel model)
    {
      _telemetry.TrackEvent("CancelApptConfirmed");
      //$"https://{Request.Host}/schedule/{model.ServiceProviderVanityUrl}"
      UserRole userRole = _httpContext.GetUserRole();
      //todo: fix
      string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      string firstName = User.Claims.First(c => c.Type == IdentityK.GivenName).Value;
      string lastName = User.Claims.First(c => c.Type == IdentityK.SurName).Value;
      string email = User.Claims.First(c => c.Type == IdentityK.Email).Value;
      string spVanityUrl = $"https://{Request.Host}/schedule/{serviceProviderVanityUrl}";
      var canceledAppt = new CancelAppointmentRequest()
      {
        UserId = userId,
        CanceledByName = $"{firstName} {lastName} ({email})",
        AppointmentId = model.AppointmentId,
        CancelationNotes = model.CancelationNotes,
        UserTimeZoneId = model.IanaTimeZoneId,
        SpVanityUrl = spVanityUrl,
        SpVanity = serviceProviderVanityUrl
      };
      CancelAppointmentResponse response = _apptRepository.CancelAppointmentAsync(canceledAppt).GetAwaiter().GetResult();
      //todo:figure out how to use, didnt work on first test expecting a false
      TempData["ApptCanceled"] = Convert.ToString(response.CanceledApptSuccess);
      TempData["RemindersCanceled"] = Convert.ToString(response.CanceledRemindersSuccess);
      TempData["GcalEventCanceled"] = Convert.ToString(response.CanceledGmailCalEvent);
      TempData["SpVanityUrl"] = spVanityUrl;
      return RedirectToAction("Canceled");
    }
    //[Authorize("SchedulingPrivacy")]
    public IActionResult Canceled()
    {
      _telemetry.TrackPageView("CancelApptConfirmed");
      return View();
    }
  }
}