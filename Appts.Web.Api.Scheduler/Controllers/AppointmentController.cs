using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Messaging.ServiceBus;
using Appts.Models.Document;
using Appts.Models.Domain;
using Appts.Models.Rest;
using Appts.Models.SendGrid;
using Appts.Models.SendGrid.Templates;
using Appts.Models.SendGrid.Requests;
using Appts.Web.Api.Scheduler.Repositories;
using Appts.Web.Api.Scheduler.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
namespace Appts.Web.Api.Scheduler.Controllers
{
  public class AppointmentController : Controller
  {
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly ICommunicationService _comm;
    private readonly ILogger<AppointmentController> _logger;
    public AppointmentController(
      IAppointmentRepository appointmentRepository,
      IServiceProviderRepository serviceProviderRepository,
      IAppointmentTypeRepository appointmentTypeRepository,
      ICommunicationService comm,
      ILogger<AppointmentController> logger)
    {
      _appointmentRepository = appointmentRepository;
      _serviceProviderRepository = serviceProviderRepository;
      _appointmentTypeRepository = appointmentTypeRepository;
      _comm = comm;
      _logger = logger;
    }
    [HttpPost("api/[controller]")]
    public void Post([FromBody] SaveApptRequest request)
    {
      Appointment appointment = request.Appointment;
      if (appointment.ClientMobile != null)
      {
        //todo: fix bug with sms sending too early
        appointment.ClientApptReminders = ScheduleClientApptReminders(appointment);
      }
      //appointment.SpApptReminders = ScheduleSpApptReminders(appointment);
      _appointmentRepository.AddAppointmentAsync(appointment).GetAwaiter().GetResult();
      //add clint appt rcrd
      if (request.CreateRelationshipIfNotExists)
      {
        //create if not exists; sp x client relationship
        _serviceProviderRepository.CreateClientXSpRelationsIfNotExists(appointment.UserId, appointment.ClientId).GetAwaiter().GetResult();
      }
      var sp = _serviceProviderRepository.GetAsync(appointment.UserId).GetAwaiter().GetResult();
      var commRequest = new SendApptScheduledRequest()
      {
        SendEmail = true,
        ClientEmail = appointment.ClientEmail,
        ClientName = appointment.ClientFirstName,
        ClientMobile = appointment.ClientMobile,
        SpName = sp.DisplayName,
        ApptSummary = appointment.ToString(),
        SpEmail = sp.Email,
        CancelUrl = $"{request.SiteBaseUrl}/appointment/cancel/{request.SpVanity}?apptId={appointment.Id}",
        //RescheduleUrl = $"{request.SiteBaseUrl}/appointment/reschedule/{request.SpVanity}?apptId={appointment.Id}",
        SpVanityUrl = request.SpVanityUrl,
        ScheduleBy = appointment.Created.ByName,
        Notes = appointment.Notes
      };
      //_comm.SendApptScheduledEmailAsync(emailRequest).GetAwaiter().GetResult();
      //determine if phone number provided
      //testing as if phn
      if (appointment.ClientMobile != null)
      { 
        commRequest.SendSms = true;
      }
      _comm.SendApptScheduledAsync(commRequest).GetAwaiter().GetResult();
    }
    List<long> ScheduleClientApptReminders(Appointment appt)
    {
      Appointment clientTz = appt;
      string displayTimeZneId;
      if (appt.ClientTimeZone != null)
      { 
        clientTz.ConvertAllTimes(appt.ClientTimeZone);
        displayTimeZneId = appt.ClientTimeZone;
      }
      else
      {
        displayTimeZneId = appt.TimeZoneId;
      }
      DateTime zonedNow = Chronotope.CreateZonedTime(displayTimeZneId);
      var reminder = new SendApptReminderRequest()
      {
        ApptSummary = appt.ToString(),
        ClientPhoneNumber = appt.ClientMobile,
        DisplayTimeZoneId = displayTimeZneId
        //Reminder1 = r,
        //Reminder2 = r2
      };
      TimeSpan timeUntil1HourReminder = clientTz.StartTime.Subtract(zonedNow.AddHours(1));
      TimeSpan timeuntil15MinuteReminder = clientTz.StartTime.Subtract(zonedNow.AddMinutes(15));
      if (timeUntil1HourReminder.TotalSeconds > 0)
      { 
        DateTime r = DateTime.UtcNow.AddSeconds(timeUntil1HourReminder.TotalSeconds);
        reminder.Reminder1 = r;
      }
      if (timeuntil15MinuteReminder.TotalSeconds > 0)
      { 
        DateTime r2 = DateTime.UtcNow.AddSeconds(timeuntil15MinuteReminder.TotalSeconds);
        reminder.Reminder2 = r2;
      }
      return _comm.SendApptReminderSmsAsync(reminder).GetAwaiter().GetResult();
    }
    //not sending sp reminders for now, saving the SMS count, sps can check the website
    //List<long> ScheduleSpApptReminders(Appointment appt)
    //{
    //  List<long> seqNo = new List<long>();
    //  var reminder1 = new SendApptReminderRequest()
    //  {
    //    ApptSummary = appt.ToString()
    //  };
    //  return _comm.SendApptReminderSmsAsync(reminder1).GetAwaiter().GetResult();
    //}
    [HttpPatch("api/[controller]/[action]")]
    public void Reschedule([FromBody] RescheduleAppointmentRequest request)
    {
      //_appointmentRepository.AddAppointmentAsync(appointment).GetAwaiter().GetResult();
      _appointmentRepository.RescheduleAppointmentAsync(request).GetAwaiter().GetResult();
    }
    /// <summary>
    /// Called by Appointment/Index page to list out appts for easy management.
    /// </summary>
    /// <param name="serviceProviderId">user id of sp</param>
    /// <param name="start">starting date range of appts</param>
    /// <param name="end">ending date range of appts</param>
    /// <param name="typeId">appt type, optional filter</param>
    /// <param name="status">appt status, Active, Canceled, optional filter</param>
    /// <param name="timezone">IANA timezone string to display dates and times in.</param>
    /// <returns></returns>
    [HttpGet("api/[controller]/[action]/{serviceProviderId}")]
    public ViewAppointmentsResponse GetViewAppointments(
      string serviceProviderId,
      DateTime start, DateTime end, UserRole userRole,
      string typeId, string status,
      string timezone = null)
    {
      //todo: add sp vanity url field
      var response = new ViewAppointmentsResponse();
      string effectiveTimeZoneId = timezone;
      if (timezone == null)
      {
        var spProfile = _serviceProviderRepository.GetAsync(serviceProviderId).GetAwaiter().GetResult();
        response.SpVanityUrl = spProfile.VanityUrl;
        response.IanaTimeZoneId = spProfile.TimeZoneId;
        effectiveTimeZoneId = response.IanaTimeZoneId;
      }
      List<Appointment> appts = _appointmentRepository.GetAppointmentsByProviderAsync(
        serviceProviderId, start, end, typeId, status)
        .GetAwaiter().GetResult();
      //// as tz differences are encountered and calculated,
      //// save into dictionary to prevent re-calculating known values
      //Dictionary<string, string> seenTimeZones = new Dictionary<string, string>();
      Appointment currentAppointment;
      List<string> apptIdsToMarkAsRead = new List<string>();
      for (int i = 0; i < appts.Count; i++)
      {
        currentAppointment = appts[i];
        apptIdsToMarkAsRead.Add(currentAppointment.Id);
        if (currentAppointment.TimeZoneId != effectiveTimeZoneId)
        {
          currentAppointment.ConvertAllTimes(effectiveTimeZoneId);
          appts[i] = currentAppointment;
        }
        //must be done on client server side (web ui server)
        //set read reciept based on user role
        //currentAppointment.SpReadReciept
      }
      response.Appointments = appts;
      /// make change after appts have been assigned to view model. intentionally show original state.
      response.AppointmentTypes = _appointmentTypeRepository.GetAppointmentTypesAsync(serviceProviderId)
        .GetAwaiter().GetResult();
      //apptIdsToMarkAsRead
      //cosmosdb to update these documents
      int nApptsMakredAsRead = 0;
      List<string> newApptIds = new List<string>();
      foreach (Appointment appointment in response.Appointments)
      {
        ///only update if neccessary, no date populated in field
        if (userRole == UserRole.Client && appointment.ClientReadReciept == null ||
          userRole == UserRole.Subscriber && appointment.SpReadReciept == null)
        { 
          _appointmentRepository.UpdateReadRecieptNoReturnAsync(appointment, userRole);
          nApptsMakredAsRead++;
          newApptIds.Add(appointment.Id);
        }
      }
      response.NewAppointments = nApptsMakredAsRead;
      response.NewApptIds = newApptIds;
      return response;
    }
    [HttpGet("api/[controller]/[action]")]
    public ViewAppointmentsResponse GetViewAppointmentsForClient(
  string clientId,
  DateTime start, DateTime end, string typeId, string status,
  string timezone = null)
    {
      //todo: add sp vanity url field
      var response = new ViewAppointmentsResponse();

      string effectiveTimeZoneId = timezone;
      if (timezone == null)
      {
        //todo: get client info instead of service provider
        //var spProfile = _serviceProviderRepository.GetAsync(serviceProviderId).GetAwaiter().GetResult();
        //response.SpVanityUrl = spProfile.VanityUrl;
        //response.IanaTimeZoneId = spProfile.TimeZoneId;
        //effectiveTimeZoneId = response.IanaTimeZoneId;
      }
      List<Appointment> appts = _appointmentRepository.GetAppointmentsByProvidersAsync(
        "", start, end, typeId, status)
        .GetAwaiter().GetResult();
      //// as tz differences are encountered and calculated,
      //// save into dictionary to prevent re-calculating known values
      //Dictionary<string, string> seenTimeZones = new Dictionary<string, string>();
      Appointment currentAppointment;
      for (int i = 0; i < appts.Count; i++)
      {
        currentAppointment = appts[i];
        if (currentAppointment.TimeZoneId != effectiveTimeZoneId)
        {
          currentAppointment.ConvertAllTimes(effectiveTimeZoneId);
          appts[i] = currentAppointment;
        }
      }
      response.Appointments = appts;
      //todo: adapt to client
      //response.AppointmentTypes = _appointmentTypeRepository.GetAppointmentTypesAsync(serviceProviderId)
      //  .GetAwaiter().GetResult();
      return response;
    }
    [HttpGet("api/[controller]/[action]/{spVanityUrl}")]
    public Appointment Detail(string spVanityUrl, string appointmentId)
    {
      string serviceProviderId = _serviceProviderRepository.GetServicProviderUserIdAsync(spVanityUrl).GetAwaiter().GetResult();
      return _appointmentRepository.GetAppointmentAsync(serviceProviderId, appointmentId).GetAwaiter().GetResult();
    }
    [HttpGet("api/[controller]/[action]/{serviceProviderVanityUrl}")]
    public GetAppointmentDetailResponse GetAppointmentToCancel(
        string serviceProviderVanityUrl,
        [FromQuery]GetAppointmentDetailRequest request)
    {
      var response = new GetAppointmentDetailResponse();
      string serviceProviderId = _serviceProviderRepository.GetServicProviderUserIdAsync(serviceProviderVanityUrl).GetAwaiter().GetResult();
      // TODO: consider moving spid to qs param instead of part of route
      request.UserId = serviceProviderId;
      response = _appointmentRepository.GetAppointmentToCancelAsync(request)
        .GetAwaiter().GetResult();
      return response;
    }
    //todo: consider deleting sequence numbers after canceled
    [HttpPatch("api/[controller]/[action]")]
    public CancelAppointmentResponse Cancel2([FromBody] CancelAppointmentRequest request)
    {
      var response = new CancelAppointmentResponse();
      try
      {
        response = _appointmentRepository.CancelAppointmentAsync(request).GetAwaiter().GetResult();
        response.CanceledApptSuccess = true;
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed to cancel appointment", ex.Message, ex.StackTrace);
      }
      try
      {
        int canceledRemindersCount = _comm.CanceledScheduledSmsLists(
          response.CanceledAppointment.ClientApptReminders, 
          response.CanceledAppointment.SpApptReminders)
          .GetAwaiter().GetResult();
        response.CanceledRemindersSuccess = true;
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed to canceled reminders", ex.Message, ex.StackTrace);
      }
      try
      {
        Appointment appt = response.CanceledAppointment;
        var apptCanceledRequest = new SendApptScheduledRequest()
        {
          ClientEmail = appt.ClientEmail,
          ClientName = $"{appt.ClientFirstName} {appt.ClientLastName}",
          //todo: retrieve sps email in repository?
          SpEmail = response.SpEmail,
          SpName = response.SpDisplayName,
          SpVanityUrl = request.SpVanityUrl,
          SendEmail = true,
          SendSms = true,
          ApptSummary = response.CanceledAppointment.ToString(),
          CancelationNotes = appt.CancelationNotes,
          CanceledBy = appt.Canceled.ByName
        };
        response.SentCancelMessages = _comm.SendApptCanceledAsync(apptCanceledRequest).GetAwaiter().GetResult();
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed to send cancel email", ex.Message, ex.StackTrace);
      }
      return response;
    }
  }
}