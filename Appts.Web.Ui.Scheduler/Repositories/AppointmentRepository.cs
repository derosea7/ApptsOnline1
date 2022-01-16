using System;
using Appts.Models.Document;
using Appts.Web;
using Appts.Web.Ui.Scheduler.Services;
using System.Threading.Tasks;
using Appts.Models.Rest;
using Microsoft.Extensions.Logging;

namespace Appts.Web.Ui.Scheduler.Repositories
{
  public class AppointmentRepository : IAppointmentRepository
  {
    private readonly IApiClient _apiClient;
    private readonly IGoogleCalendarClient _googleCalendarClient;
    private readonly ILogger<AppointmentRepository> _logger;
    public AppointmentRepository(IApiClient apiClient, IGoogleCalendarClient googleCalendarClient,
      ILogger<AppointmentRepository> logger)
    {
      _apiClient = apiClient;
      _googleCalendarClient = googleCalendarClient;
      _logger = logger;
    }
    public async Task<CancelAppointmentResponse> CancelAppointmentAsync(CancelAppointmentRequest request)
    {
      CancelAppointmentResponse response = await _apiClient.PatchAsync<CancelAppointmentRequest, CancelAppointmentResponse>(
        request, "/api/appointment/cancel2");
      Appointment canceledAppt = response.CanceledAppointment;
      //BUG: canceledAppt.UserId is being set incorrectly upstream
      _googleCalendarClient.Initialize(canceledAppt.UserId);
      try
      {
        await _googleCalendarClient.CancelCalendarEvent(canceledAppt.GcalEventId,
          canceledAppt.StartTime, canceledAppt.EndTime);
        response.CanceledGmailCalEvent = true;
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed to cancel Google calendar event", ex.Message, ex.StackTrace);
      }
      return response;
    }
    public async Task<GetAppointmentDetailResponse> GetAppointmentToCancel(
      GetAppointmentDetailRequest request)
    {
      return await _apiClient.GetAsync<GetAppointmentDetailResponse>(
        //$"/api/appointment/detail2/{request.UserId}?apptId={request.ApptId}");
        $"/api/appointment/getappointmenttocancel/{request.SpVanityUrl}?apptId={request.ApptId}");
    }

    public async Task<ViewAppointmentsResponse> GetViewAppointmentsAsync(
      ViewAppointmentsRequest request)
    {
      var qs = $"start={request.Start:yyyy-MM-dd}&end={request.End:yyyy-MM-dd}";

      // append optional parameters
      qs += request.ApptTypeId == null ? "" : $"&typeId={request.ApptTypeId}";
      qs += request.ApptStatus == null ? "" : $"&status={request.ApptStatus}";
      qs += request.IanaTimeZoneId == null ? "" : $"&timezone={System.Net.WebUtility.UrlEncode(request.IanaTimeZoneId)}";
      qs += request.UserRole == null ? "" : $"&userRole={request.UserRole}";
      string endpoint = $"/api/appointment/GetViewAppointments/{request.UserId}?{qs}";

      return await _apiClient.GetAsync<ViewAppointmentsResponse>(endpoint);
    }

    public async Task<Appointment> GetAppointmentAsync(string spVanityUrl, string apptId)
    {
      //await SetToken();
      string endpoint = $"/api/appointment/detail/{spVanityUrl}?appointmentId={apptId}";
      return await _apiClient.GetAsync<Appointment>(endpoint);
    }

    //public async Task Reschedule(RescheduleAppointmentRequest request)
    //{
    //  await _apiClient.PatchNoReturnAsync<RescheduleAppointmentRequest>(
    //    request, $"/api/appointment/reschedule");
    //}
  }
}
