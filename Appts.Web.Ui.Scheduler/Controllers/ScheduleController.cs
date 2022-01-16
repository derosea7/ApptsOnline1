using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Appts.Models.View;
using Appts.Models.Api;
using Appts.Models.Domain;
using Appts.Models.Rest;
using Appts.Web.Ui.Scheduler.Repositories;
using Appts.Models.Document;
using Appts.Common.Constants;
using Appts.Web.Ui.Scheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Appts.Web.Ui.Scheduler.ViewModels;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  /// <summary>
  /// Facilitates user scheduling an appointment with service provider.
  /// User can be scheduling in the following User Roles:
  ///   1) Anonymous
  ///   2) Registered user
  ///   3) Whitelisted user
  ///   4) Service Provider
  /// </summary>
  public class ScheduleController : Controller
  {
    private readonly IApiClient _apiClient;
    private readonly IGoogleCalendarClient _googleCalendarClient;
    private readonly IAppointmentRepository _apptRepo;
    private readonly IHttpContextResolverService _httpContext;
    private TelemetryClient _telemetry;
    public ScheduleController(
      IApiClient apiClient, 
      IGoogleCalendarClient googleCalendarClient, 
      IAppointmentRepository apptRepo, 
      IHttpContextResolverService httpContext,
      TelemetryClient telemetry)
    {
      _apiClient = apiClient;
      _googleCalendarClient = googleCalendarClient;
      _apptRepo = apptRepo;
      _httpContext = httpContext;
      _telemetry = telemetry;
    }
    /// <summary>
    /// First entry-point for clients scheduling appointment from a URL.
    /// Get Service Provider info from url, and their active appointment types.
    /// TODO: if sp not found at the url, show Not Found page
    /// </summary>
    /// <param name="serviceProviderVanityUrl">Url identifying a service provider.</param>
    /// <param name="rescheduleApptId">
    /// Optional. If provided, indicates this is a rescheduling scenario, and loads the
    /// existing appointment by the given id.
    /// </param>
    /// <returns>View to let user choose appointment type</returns>
    [Authorize("SchedulingPrivacy")]
    [HttpGet("[controller]/{serviceProviderVanityUrl}")]
    public IActionResult Get(string serviceProviderVanityUrl, 
      string rescheduleApptId = null, string clientId = null)
    {
      _telemetry.TrackEvent("ScheduleNewApptInitiated");
      _telemetry.TrackPageView("NewApptScheduler");
      var model = new ScheduleAppointmentViewModel();
      UserRole userRole = _httpContext.GetUserRole();
      if (rescheduleApptId == null)
      {
        model = GetSchedulingView(serviceProviderVanityUrl, userRole, clientId);
      }
      else
      {
        model = GetReschedulingView(serviceProviderVanityUrl, rescheduleApptId, clientId);
      }
      if (model.ServiceProviderId == null)
      {
        TempData["NameOfSpSearched"] = serviceProviderVanityUrl;
        return RedirectToAction("ServiceProviderNotFound", "Schedule");
      }
      return View("NewAppointment", model);
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult ServiceProviderNotFound()
    {
      _telemetry.TrackEvent("SpNotFoundAtUrl");
      _telemetry.TrackPageView("SpNotFoundAtUrl");
      return View();
    }
    /// <summary>
    /// Gets the initial info required to schedule an appointment.
    /// </summary>
    /// <param name="serviceProviderVanityUrl">
    /// Url that identifies a service provider.
    /// </param>
    /// <returns>Populated view model</returns>
    private ScheduleAppointmentViewModel GetSchedulingView(
      string serviceProviderVanityUrl, UserRole userRole, string clientId = null)
    {
      var request = new InitialSchedulingInfoRequest()
      {
        ServiceProviderVanityUrl = serviceProviderVanityUrl,
        UserId = _httpContext.GetUserId(),
        UserRole = userRole
      };
      var response = _apiClient.PostAsync<InitialSchedulingInfoRequest, InitialSchedulingInfoResponse>
        (request, $"/api/Availability/InitialSchedulingInfo").GetAwaiter().GetResult();
      if (response.ServiceProvider == null)
      {
        return new ScheduleAppointmentViewModel();
      }
      SchedulerRole schedulerRole = response.SchedulerRole;
      string clientUserId = "";
      if (schedulerRole == SchedulerRole.Client)
      {
        clientUserId = _httpContext.GetUserId();
      }
      var model = new ScheduleAppointmentViewModel()
      {
        BusinessDisplayName = response.ServiceProvider.DisplayName,
        BusinessTimeZone = response.ServiceProvider.TimeZoneId,
        ServiceProviderId = response.ServiceProvider.UserId,
        AppointmentTypes2 = response.AppointmentTypes,
        SchedulerRole = schedulerRole,
        SpVanityUrl = response.ServiceProvider.VanityUrl
      };
      switch (model.SchedulerRole)
      {
        case SchedulerRole.Undefined:
          break;
        case SchedulerRole.AnonymousScheduler:
          break;
        case SchedulerRole.Client:
          model.ClientFirstName = response.ClientInfo.FirstName;
          model.ClientEmail = response.ClientInfo.Email;
          model.ClientLastName = response.ClientInfo.LastName;
          model.ClientTimeZone = response.ClientInfo.TimeZoneId;
          model.ClientMobilePhone = response.ClientInfo.MobilePhone;
          model.ClientAddress = response.ClientInfo.Address;
          model.ClientAddress2 = response.ClientInfo.Address2;
          model.ClientCity = response.ClientInfo.City;
          model.ClientStateCode = response.ClientInfo.StateCode;
          model.ClientZipCode = response.ClientInfo.ZipCode;
          break;
        case SchedulerRole.Subscriber:
          break;
        case SchedulerRole.FreeSubscriber:
          break;
        case SchedulerRole.SelfScheduler:
          break;
        default:
          break;
      }
      if (userRole == UserRole.Subscriber)
      {
        // get list of clients and present as selection
        model.ClientList = response.ClientList;
      }
      return model;
    }
    /// <summary>
    /// Get the initial info required to re-schedule an appointment (namely the appointment).
    /// </summary>
    /// <param name="serviceProviderVanityUrl">Url that identifies a service provider.</param>
    /// <param name="appointmentId"></param>
    /// <returns>Appointment and times that can be used to reschedule it.</returns>
    private ScheduleAppointmentViewModel GetReschedulingView(string serviceProviderVanityUrl, 
      string appointmentId, string clientId = null)
    {
      var request = new InitialReschedulingInfoRequest()
      {
        ServiceProviderVanityUrl = serviceProviderVanityUrl,
        AppointmentIdToReschedule = appointmentId
      };
      string endpoint = $"/api/Availability/InitialReschedulingInfo";
      endpoint += $"?ServiceProviderVanityUrl={serviceProviderVanityUrl}";
      endpoint += $"&AppointmentIdToReschedule={appointmentId}";

      var response = _apiClient.GetAsync<InitialReschedulingInfoResponse>(endpoint)
        .GetAwaiter().GetResult();
      if (response.ServiceProvider == null)
      {
        // redirect to error or something; sp not found at that url
      }
      var model = new ScheduleAppointmentViewModel()
      {
        Reschedule = true,
        BusinessDisplayName = response.ServiceProvider.DisplayName,
        BusinessTimeZone = response.ServiceProvider.TimeZoneId,
        ServiceProviderId = response.ServiceProvider.UserId,
        //AppointmentTypes2 = response.AppointmentTypes,
        SchedulerRole = GetSchedulerRole(response.ServiceProvider.UserId),
        Appointment = new Appointment()
        {
          AppointmentTypeId = response.AppointmentToReschedule.AppointmentTypeId
        },
        AppointmentDuration = response.AppointmentToReschedule.EndTime.Subtract(
          response.AppointmentToReschedule.StartTime),
        AppointmentTypeBreif = response.AppointmentToReschedule.AppointmentTypeBreif,
        ClientEmail = response.AppointmentToReschedule.ClientEmail,
        ClientFirstName = response.AppointmentToReschedule.ClientFirstName,
        ClientLastName = response.AppointmentToReschedule.ClientLastName,
        ClientTimeZone = response.AppointmentToReschedule.ClientTimeZone,
        AppointmentIdToReschedule = appointmentId
      };
      return model;
    }
    private SchedulerRole GetSchedulerRole(string requestServiceProviderId = null)
    {
      SchedulerRole schedulerRole;
      if (!User.Identity.IsAuthenticated)
      {
        schedulerRole = SchedulerRole.AnonymousScheduler;
      }
      else
      {
        string currentUserId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;

        string[] whitelistTypeValues = new string[] { "Client", "Sp" };
        string userType = User.Claims
          .Where(c => whitelistTypeValues.Contains(c.Value))
          .FirstOrDefault(c => c.Type == IdentityK.Type).Value;
        //string userType = User.Claims.FirstOrDefault(c => c.Type == IdentityK.Type).Value;
        if ((userType == "Sp") && currentUserId == requestServiceProviderId)
        {
          schedulerRole = SchedulerRole.Subscriber;
        }
        else if (userType == "Client" || (userType == "Sp" && currentUserId != requestServiceProviderId))
        {
          schedulerRole = SchedulerRole.Client;
        }
        else
        {
          schedulerRole = SchedulerRole.Undefined;
        }
      }
      return schedulerRole;
    }
    [HttpPost("[controller]/[action]")]
    public IActionResult RescheduleAppointment(ScheduleAppointmentViewModel model)
    {
      var s = ModelState.ValidationState;
      if (!ModelState.IsValid)
      {
        return View("ScheduleAppointment", model);
      }
      //var request = new RescheduleAppointmentRequest();
      //_appointmentRepository.RescheduleAppointmentAsync(
      //  request);
      var request = new RescheduleAppointmentRequest()
      {
        ServiceProviderId = model.ServiceProviderId,
        AppointmentIdToReschedule = model.AppointmentIdToReschedule,
        NewStartTime = model.StartTime,
        NewEndTime = model.EndTime,
        RescheduleNotes = ""
      };
      _apiClient.PatchNoReturnAsync<RescheduleAppointmentRequest>(
        request, $"/api/appointment/reschedule").GetAwaiter().GetResult();
      return RedirectToAction("Success", "Schedule", new { businessNameUrl = "Cindi_SLPA" });
    }
    [HttpPost]
    [Route("[controller]/[action]")]
    //public void SaveAppointment(Appointment appointment)
    public IActionResult SaveAppointment(ScheduleAppointmentViewModel model)
    {
      _telemetry.TrackEvent("SavingAppt");
      var s = ModelState.ValidationState;
      SchedulerRole schedulerRole = GetSchedulerRole(model.ServiceProviderId);
      ValidateClientFirstName(schedulerRole, model);
      if (!ModelState.IsValid)
      {
        model.SchedulerRole = schedulerRole;
        model.IsGetFromInvalidPost = true;

        //todo: populate this spVanityUrl when posting an appoitnment
        return RedirectToAction(model.SpVanityUrl, "Schedule");
      }
      var newModel = model;
      model = new ScheduleAppointmentViewModel()
      {
        Appointment = new Appointment()
        {
          TimeZoneId = newModel.BusinessTimeZone,
          ClientEmail = newModel.ClientEmail,
          ClientFirstName = newModel.ClientFirstName,
          ClientLastName = newModel.ClientLastName,
          ClientTimeZone = newModel.ClientTimeZone,
          ClientMobile = newModel.ClientMobilePhone,
          //ClientUserRole = schedulerRole,
          StartTime = newModel.StartTime,
          EndTime = newModel.EndTime,
          UserId = newModel.ServiceProviderId,
          AppointmentTypeId = newModel.AppointmentTypeId,
          AppointmentTypeBreif = newModel.AppointmentTypeBreif,
          Notes = newModel.Notes
        }
      };
      if (newModel.Location == "we_call" || newModel.Location == "customer_specifies")
      {
        model.Appointment.LocationDetails = newModel.LocationDetails;
      }
      // if is anon, we've collected email + name on the form
      // if is subscriber, we've collected email + name on the form
      string email = "";
      string fname = "";
      string lname = "";
      string clientid = "";
      switch (schedulerRole)
      {
        case SchedulerRole.Client:
          email = User.Claims.First(e => e.Type == IdentityK.Email).Value;
          fname = User.Claims.First(c => c.Type == IdentityK.GivenName).Value;
          lname = User.Claims.First(c => c.Type == IdentityK.SurName).Value;
          clientid = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
          model.Appointment.ClientId = clientid;
          model.Appointment.ClientEmail = email;
          model.Appointment.ClientFirstName = fname;
          model.Appointment.ClientLastName = lname;
          model.Appointment.Created = new AuditTrail()
          {
            On = Chronotope.CreateZonedTime(model.Appointment.TimeZoneId),
            ById = model.Appointment.ClientId,
            ByEmail = model.Appointment.ClientEmail,
            ByName = $"{model.Appointment.ClientFirstName} {model.Appointment.ClientLastName} ({model.Appointment.ClientEmail})"
          };
          break;
        case SchedulerRole.AnonymousScheduler:
          model.Appointment.Created = new AuditTrail()
          {
            On = Chronotope.CreateZonedTime(model.Appointment.TimeZoneId),
            ByEmail = model.Appointment.ClientEmail,
            ByName = $"{model.Appointment.ClientFirstName} {model.Appointment.ClientLastName} ({model.Appointment.ClientEmail})"
          };
          break;
        case SchedulerRole.Subscriber:
          email = User.Claims.First(e => e.Type == IdentityK.Email).Value;
          fname = User.Claims.First(c => c.Type == IdentityK.GivenName).Value;
          lname = User.Claims.First(c => c.Type == IdentityK.SurName).Value;
          string spid = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
          model.Appointment.Created = new AuditTrail()
          {
            On = Chronotope.CreateZonedTime(model.Appointment.TimeZoneId),
            ById = spid,
            ByEmail = email,
            ByName = $"{fname} {lname} ({email})"
          };
          break;
        default:
          break;
      }
      //todo: save guid as string, pass on during redirection to redisplay newly added appt
      string newapptguid = Guid.NewGuid().ToString();
      model.Appointment.Id = newapptguid;
      model.Appointment.Status = EntityStatus.Active;
      // TODO: consider more efficient way of handling this whole situation
      bool spHasConnectedGoolgeCalendar = _apiClient.GetAsync<bool>(
        $"/api/calendars/HasGoogleCalendar/{model.Appointment.UserId}")
        .GetAwaiter().GetResult();
      if (spHasConnectedGoolgeCalendar)// service provider has connected calendar
      {
        try
        {
          _googleCalendarClient.Initialize(model.Appointment.UserId);
          // pass spvanityurl for cancel url
          string eventId = _googleCalendarClient.CreateCalendarEvent(model.Appointment, newModel.BusinessDisplayName).GetAwaiter().GetResult();
          model.Appointment.GcalEventId = eventId;
        }
        catch (Exception)
        {
          // log?
          //throw;
        }
      }
      // would schedule appt in this system first, but easier to
      // get the event id from gcal first then save that off with other appt data
      var appt = model.Appointment;
      string baseUrl = $"https://{Request.Host}";///schedule/{serviceProviderVanityUrl}";
      var request = new SaveApptRequest()
      {
        Appointment = model.Appointment,
        SpVanityUrl = $"{baseUrl}/schedule/{newModel.SpVanityUrl}",
        SpVanity = newModel.SpVanityUrl,
        SiteBaseUrl = baseUrl
      };
      if (model.Appointment.ClientId != null && schedulerRole != SchedulerRole.AnonymousScheduler)
      {
        request.CreateRelationshipIfNotExists = true;
      }
      _apiClient.PostNoReturnAsync<SaveApptRequest>(request, $"/api/Appointment").GetAwaiter().GetResult();
      return RedirectToAction("Success", "Schedule", new { serviceProviderUrl = newModel.SpVanityUrl, apptId = newapptguid });
    }
    /// <summary>
    /// Add error to model state if anonymous user or subscriber
    /// did not provider client first name.
    /// 
    /// Client information is required for some user roles,
    /// but not when the client is logged in as themselves.
    /// </summary>
    /// <param name="userRole">Users current role.</param>
    /// <param name="model">values from form</param>
    private void ValidateClientFirstName(SchedulerRole schedulerRole, ScheduleAppointmentViewModel model)
    {
      switch (schedulerRole)
      {
        case SchedulerRole.AnonymousScheduler:
          if (string.IsNullOrEmpty(model.ClientFirstName))
          {
            ModelState.AddModelError("ClientFirstName", "First name is required");
          }
          break;
        case SchedulerRole.Client:
          break;
        case SchedulerRole.Subscriber:
          if (string.IsNullOrEmpty(model.ClientFirstName))
          {
            ModelState.AddModelError("ClientFirstName", "Clients first name is required");
          }
          break;
        default:
          break;
      }
    }
    // GET: /<controller>/
    [Route("[controller]/[action]")]
    public IActionResult Success(string serviceProviderUrl, string apptId)
    {
      _telemetry.TrackEvent("ApptSaved");
      _telemetry.TrackPageView("ApptScheduledSuccessfully");
      var appt = _apptRepo.GetAppointmentAsync(serviceProviderUrl, apptId).GetAwaiter().GetResult();
      var schedulerRole = GetSchedulerRole(appt.UserId);
      var vm = new ScheduleAppointmentViewModel()
      {
        Appointment = appt,
        SpVanityUrl = serviceProviderUrl,
        SchedulerRole = schedulerRole
      };
      return View(vm);
    }
    [Route("[controller]/[action]")]
    public IActionResult Calendar(DateTime? start = null, DateTime? end = null)
    {
      //string userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      //DateTime startOrDefault = (DateTime)((start == null) ? DateTime.Today.AddDays(-1) : start);
      //DateTime endOrDefault = (DateTime)((end == null) ? DateTime.Today.AddDays(5) : end);
      //var events = GetApptsForCalendar(startOrDefault, endOrDefault, userId);
      //ApptsCalendarViewModel vm = new ApptsCalendarViewModel()
      //{
      //  //for consumption by fullcalendar.io
      //  CalendarEvents = JsonConvert.SerializeObject(events)
      //};
      var vm = new ApptsCalendarViewModel() { };
      return View(vm);
    }
    /// <summary>
    /// endpoint used by this ctl and dashboard, where calendars are displayed
    /// </summary>
    /// <param name="start">start date of event data</param>
    /// <param name="end">end date of event data</param>
    /// <param name="userId">service provider user id of event data</param>
    /// <returns>list of event data</returns>
    [Route("[controller]/[action]")]
    public List<CalendarEvent> GetApptsForCalendar(DateTime start, DateTime end,
      string userId = null)
    {
      if (userId == null)
        userId = User.Claims.First(c => c.Type == IdentityK.NameIdentifier).Value;
      var request = new ViewAppointmentsRequest()
      {
        UserId = userId,
        Start = start,
        End = end
        //ApptTypeId = AppointmentTypeId,
        //ApptStatus = AppointmentStatus,
        //IanaTimeZoneId = tz,
      };
      var response = _apptRepo.GetViewAppointmentsAsync(request).GetAwaiter().GetResult();
      List<Appointment> appts = response.Appointments;
      List<CalendarEvent> events = ConvertApptsToCalendarEvents(appts);
      return events;
    }
    private List<CalendarEvent> ConvertApptsToCalendarEvents(List<Appointment> appts)
    {
      CalendarEvent e = new CalendarEvent();
      List<CalendarEvent> es = new List<CalendarEvent>();
      foreach (Appointment appt in appts)
      {
        es.Add(new CalendarEvent()
        { 
          Id = appt.Id,
          StartDate = appt.StartTime,
          EndDate = appt.EndTime,
          Description = $"{appt.AppointmentTypeBreif} | {appt.ClientEmail}"
        });
      }
      return es;
    }
  }
}