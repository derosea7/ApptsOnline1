using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Dal.Cosmos;
using Appts.Models.Document;
using Appts.Models.Domain;
using Appts.Models.Rest;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public class AppointmentRepository : IAppointmentRepository
  {
    private readonly IDb _db;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    public AppointmentRepository(
      IDb db, IServiceProviderRepository serviceProviderRepository)
    {
      _db = db;
      _serviceProviderRepository = serviceProviderRepository;
    }
    //used by providers
    public async Task<List<Appointment>> GetAppointmentsByProviderAsync(
      string providerUserId, DateTime start, DateTime end,
      string typeId = null, string status = null)
    {
      string sql = $@"
      select * from c
      where 
        c.userId = @providerUserId
        and c.entityType = 'Appointment'
        and c.startTime >= @start
        and c.startTime <= @end
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@providerUserId", providerUserId),
        new KeyValuePair<string, string>("@start", $"{start:yyyy-MM-dd}"),
        new KeyValuePair<string, string>("@end", $"{end.AddDays(1):yyyy-MM-dd}")
      };
      if (typeId != null)
      {
        sql += " and c.appointmentTypeId = @typeId ";
        parameters.Add(new KeyValuePair<string, string>("@typeId", typeId));
      }
      if (status != null)
      {
        sql += " and c.status = @status ";
        parameters.Add(new KeyValuePair<string, string>("@status", status));
      }
      return await _db.GetMultipeAsync<Appointment>(sql, parameters);
    }
    //used by clients who may have multiple providers
    public async Task<List<Appointment>> GetAppointmentsByProvidersAsync(
      string providerUserIdList, DateTime start, DateTime end,
      string typeId = null, string status = null)
    {
      string sql = $@"
      select * from c
      where 
        c.userId in ({providerUserIdList})
        and c.entityType = 'Appointment'
        and c.startTime >= @start
        and c.startTime <= @end
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@start", $"{start:yyyy-MM-dd}"),
        new KeyValuePair<string, string>("@end", $"{end.AddDays(1):yyyy-MM-dd}")
      };
      if (typeId != null)
      {
        sql += " and c.appointmentTypeId = @typeId ";
        parameters.Add(new KeyValuePair<string, string>("@typeId", typeId));
      }
      if (status != null)
      {
        sql += " and c.status = @status ";
        parameters.Add(new KeyValuePair<string, string>("@status", status));
      }
      return await _db.GetMultipeAsync<Appointment>(sql, parameters);
    }
    public async Task AddAppointmentAsync(Appointment appointment)
    {
      await _db.CreateNoReturnAsync(appointment);
    }
    public async Task<Appointment> GetAppointmentAsync(string serviceProviderId, string appointmentId)
    {
      string sql = $@"
      select * from c where c.userId = @userId
      and c.entityType = 'Appointment' and c.id = @apptId
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", serviceProviderId),
        new KeyValuePair<string, string>("@apptId", appointmentId),
      };
      return await _db.GetSingleAsync<Appointment>(sql, parameters);
    }
    public async Task UpdateReadRecieptNoReturnAsync(Appointment appointment, UserRole userRole)
    {
      var originalAppt = await GetAppointmentAsync(appointment.UserId, appointment.Id);
      //todo: create input arg with timezoned now, and use that as read reciept
      if (userRole == UserRole.Subscriber)
      { 
        originalAppt.SpReadReciept = DateTime.UtcNow;    
      }
      else if (userRole == UserRole.Client)
      {
        originalAppt.ClientReadReciept = DateTime.UtcNow;
      }
      await _db.ReplaceAsync<Appointment>(originalAppt);
    }
    public async Task<CancelAppointmentResponse> CancelAppointmentAsync(CancelAppointmentRequest request)
    {
      string spid = await _serviceProviderRepository.GetServicProviderUserIdAsync(request.SpVanity);
      var apptToCancel = GetAppointmentAsync(spid, request.AppointmentId)
          .GetAwaiter().GetResult();
      apptToCancel.Status = EntityStatus.Canceled;
      apptToCancel.Canceled = new AuditTrail()
      {
        On = Chronotope.CreateZonedTime(apptToCancel.TimeZoneId),
        ById = request.UserId,
        ByName = request.CanceledByName
      };
      apptToCancel.Modified = apptToCancel.Canceled;
      apptToCancel.CancelationNotes = request.CancelationNotes;
      var canceledAppt = await _db.ReplaceAsync<Appointment>(apptToCancel);
      var response = new CancelAppointmentResponse()
      { 
        CanceledAppointment = canceledAppt
      };
      var sp = await _serviceProviderRepository.GetAsync(canceledAppt.UserId);
      //sp.DisplayName;
      response.SpDisplayName = sp.DisplayName;
      response.SpEmail = sp.Email;
      return response;
    }
    public async Task RescheduleAppointmentAsync(RescheduleAppointmentRequest request)
    {
      var apptToReschedule = await GetAppointmentAsync(
        request.ServiceProviderId, request.AppointmentIdToReschedule);
      apptToReschedule.StartTime = request.NewStartTime;
      apptToReschedule.EndTime = request.NewEndTime;
      apptToReschedule.Rescheduled = new AuditTrail()
      {
        On = Chronotope.CreateZonedTime(apptToReschedule.TimeZoneId),
        ById = "need this",
        ByName = "need this"
      };
      apptToReschedule.Modified = apptToReschedule.Rescheduled;
      await _db.ReplaceNoReturnAsync(apptToReschedule);
    }
    public async Task<GetAppointmentDetailResponse> GetAppointmentToCancelAsync(
      GetAppointmentDetailRequest request)
    {
      var response = new GetAppointmentDetailResponse();
      response.Appointment = await GetAppointmentAsync(request.UserId, request.ApptId);
      response.IanaTimeZoneId = await _serviceProviderRepository
        .GetTimeZoneIdAsync(request.UserId);
      return response;
    }
  }
}