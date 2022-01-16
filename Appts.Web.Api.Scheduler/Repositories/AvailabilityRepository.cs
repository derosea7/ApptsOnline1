using Appts.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Dal.Cosmos;
using Appts.Models.Document;
using Appts.Models.Domain;
using Appts.Models.Document.Views;

namespace Appts.Web.Api.Scheduler.Repositories
{
  public class AvailabilityRepository : IAvailabilityRepository
  {
    private readonly IDb _db;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceProviderRepository _serviceProviderRepository;
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    private readonly IClientRepository _clientRepository;
    public AvailabilityRepository(
      IDb db,
      IAppointmentRepository appointmentRepository,
      IServiceProviderRepository serviceProviderRepository,
      IAppointmentTypeRepository appointmentTypeRepository,
      IClientRepository clientRepository)
    {
      _db = db;
      _appointmentRepository = appointmentRepository;
      _serviceProviderRepository = serviceProviderRepository;
      _appointmentTypeRepository = appointmentTypeRepository;
      _clientRepository = clientRepository;
    }
    /// <summary>
    /// Get service provider and their associated active appt types.
    /// </summary>
    /// <param name="serviceProviderVanityUrl"></param>
    /// <returns></returns>
    public async Task<InitialSchedulingInfoResponse> GetInitialSchedulingInfoAsync(
      //string serviceProviderVanityUrl, UserRole userRole, SchedulerRole schedulerRole, 
      //string clientUserId = null)
      InitialSchedulingInfoRequest request)
    {
      string serviceProviderId = await _serviceProviderRepository
        .GetServicProviderUserIdAsync(request.ServiceProviderVanityUrl);
      if (serviceProviderId == null)
      {
        return new InitialSchedulingInfoResponse()
        {
          // nothing indicates to ui that sp not found
        };
      }
      GetServiceProviderResponse spResponse = await _serviceProviderRepository.GetAsync(serviceProviderId);
      var response = new InitialSchedulingInfoResponse()
      {
        ServiceProvider = new ServiceProviderDocument()
        {
          DisplayName = spResponse.DisplayName,
          TimeZoneId = spResponse.TimeZoneId,
          UserId = serviceProviderId,
          VanityUrl = spResponse.VanityUrl
        },
        AppointmentTypes = await _appointmentTypeRepository.GetAppointmentTypesAsync(serviceProviderId, true)
      };
      // lookup client info if user role is client
      ClientDoc client = new ClientDoc();
      //if (userRole == UserRole.Client && clientUserId != null)
      //if (schedulerRole == SchedulerRole.Client && clientUserId != null)
      //{
      //  // client repo
      //  client = await _clientRepository.GetClientAsync(clientUserId);
      //  response.ClientInfo = client;
      //}
      response.SchedulerRole = SchedulerRole.Undefined;
      if (request.UserRole == UserRole.Subscriber && request.UserId == serviceProviderId)
      {
        response.SchedulerRole = SchedulerRole.Subscriber;
      }
      else if (request.UserRole == UserRole.Client)
      {
        response.SchedulerRole = SchedulerRole.Client;
      }
      if (response.SchedulerRole == SchedulerRole.Subscriber)
      {
        List<string> clientIdsList = await _clientRepository.GetClientIdListAsync(serviceProviderId);
        if (clientIdsList.Count > 0)
        { 
          response.ClientList = await _clientRepository.GetClientsAsync(serviceProviderId, clientIdsList);
        }
      }
      else if (request.UserRole == UserRole.AnonymousScheduler)
      {
        response.SchedulerRole = SchedulerRole.AnonymousScheduler;
      }
      else
      {
        // subscriber acting as client for some other sp
        response.SchedulerRole = SchedulerRole.Client;
        client = await _clientRepository.GetClientAsync(request.UserId);
        response.ClientInfo = client;      
      }
      return response;
    }
    public async Task<InitialReschedulingInfoResponse> InitialReschedulingInfoAsync(
      InitialReschedulingInfoRequest request)
    {
      string serviceProviderId = await _serviceProviderRepository
        .GetServicProviderUserIdAsync(request.ServiceProviderVanityUrl);
      if (serviceProviderId == null)
      {
        return new InitialReschedulingInfoResponse()
        {
          // nothing indicates to ui that sp not found
        };
      }
      GetServiceProviderResponse spResponse = await _serviceProviderRepository.GetAsync(serviceProviderId);

      // now try to find target appt using service provider id
      // if cant find it, return that and do no futhur proc
      //var apptToReschedule = await GetAppointmentAsync(
      //  business.UserId, request.AppointmentIdToReschedule);

      var apptToReschedule = await _appointmentRepository.GetAppointmentAsync(
        serviceProviderId, request.AppointmentIdToReschedule);
      var response = new InitialReschedulingInfoResponse()
      {
        ServiceProvider = new ServiceProviderDocument()
        {
          DisplayName = spResponse.DisplayName,
          Email = spResponse.Email,
          TimeZoneId = spResponse.TimeZoneId,
          UserId = serviceProviderId,
          VanityUrl = spResponse.VanityUrl
        },
        AppointmentToReschedule = apptToReschedule
      };
      return response;
    }
    public async Task DeletePeriodAsync(string userId, string periodId)
    {
      await _db.DeleteNoReturnAsync<AvailabilityPeriod>(periodId, userId);
    }
    /// <summary>
    /// Main function of scheduler; calculate available time slots given an appointment type
    /// and date range.
    /// </summary>
    /// <param name="request">
    /// 
    /// </param>
    /// <returns>
    /// 
    /// </returns>
    public async Task<GetAvailabilityForSchedulingResponse> 
      GetAvailabilityForSchedulingAsync(
      GetAvailabilityForSchedulingRequest request)
    {
      var response = new GetAvailabilityForSchedulingResponse();
      //this call here requires even the deleted appt types
      List<AppointmentType> appointmentTypes =
        await _appointmentTypeRepository.GetAppointmentTypesAsync(request.ServiceProviderId, false, true);
      AppointmentType requestedType = appointmentTypes.Where(t => t.Id == request.AppointmentTypeId).FirstOrDefault();
      string serviceProviderTimeZoneId = await _serviceProviderRepository.GetTimeZoneIdAsync(request.ServiceProviderId);
      int queryRange;
      DateTime end;
      if (!request.OneWeekOnly)
      {
        queryRange = 30;
        end = request.StartDateRange.AddDays(queryRange);
      }
      else
      {
        queryRange = 7;
        end = request.StartDateRange.AddDays(queryRange);
      }
      List<Appointment> appointments = await _appointmentRepository.GetAppointmentsByProviderAsync(
        request.ServiceProviderId, request.StartDateRange, end, null, "Active");
      List<AvailabilityPeriod> availabilityPeriods = await GetAvailabilityForClient(
        request.ServiceProviderId, request.StartDateRange, end);
      // create recurring appointments here
      // add to appointment list
      var recurringAppts = new List<Appointment>();
      foreach (Appointment appointment in appointments)
      {
        //RRULE.
        if (!string.IsNullOrEmpty(appointment.RRULE))
        {
          // generate a list of appointments, given the RRULE,
          // up until a specified end.
          // essentially expand the occurrences to create concrete
          // instances
          //RRULE.GenerateAppointments(appointment.RRULE, end);
        }
      }
      foreach (Appointment appointment in appointments)
      {
        if (appointment.TimeZoneId != serviceProviderTimeZoneId)
        {
          appointment.StartTime = Chronotope.ConvertTimeZones(appointment.StartTime, appointment.TimeZoneId, serviceProviderTimeZoneId);
          appointment.EndTime = Chronotope.ConvertTimeZones(appointment.EndTime, appointment.TimeZoneId, serviceProviderTimeZoneId);
        }
      }
      AppointmentType type;
      foreach (Appointment appointment in appointments)
      {
        type = appointmentTypes.Where(t => t.Id == appointment.AppointmentTypeId).FirstOrDefault();
        appointment.AppointmentType = new AppointmentType()
        {
          BufferAfter = type.BufferAfter,
          BufferBefore = type.BufferBefore
        };
      }
      var period = new AvailabilityPeriod();
      period.CoalescePeriods(availabilityPeriods, request.StartDateRange, queryRange);
      period.RemoveAllScheduledBlocks(appointments);
      period.RemoveBlocksWithoutEnoughTime(requestedType, request.StartDateRange);
      // availability ordered from least to greatest in period, so first 7 are earliest 7
      // filter out days without availability
      var availableDate = new List<DateTime>();
      foreach (Availability availability in period.Availability)
      {
        if (availability.Blocks != null)
        {
          availableDate.Add((DateTime)availability.ConcreteDate);
        }
      }
      List<Availability> availabilities = new List<Availability>();
      for (int i = 0; i < (availableDate.Count > 7 ? 7 : availableDate.Count); i++)
      {
        availabilities.Add(period.Availability.Where(a => a.ConcreteDate == availableDate[i]).FirstOrDefault());
      }
      response.Period = new AvailabilityPeriod()
      {
        Availability = availabilities
      };
      response.DaysWithAvailability = availableDate;
      return response;
    }
    /// <summary>
    /// Retrieve availability documents in a given range.
    /// </summary>
    /// <param name="userId">Service provider id.</param>
    /// <param name="startRangeDate">Retrieve availability active on or after this date.</param>
    /// <param name="endRangeDate">Retreive availability active on or before this date.</param>
    /// <returns></returns>
    public async Task<List<AvailabilityPeriod>> GetAvailabilityForClient(string userId, DateTime startRangeDate, DateTime endRangeDate)
    {
      string sqlFormat = "yyyy-MM-dd";
      string start = startRangeDate.ToString(sqlFormat);
      string end = endRangeDate.ToString(sqlFormat);
      //string start = startRangeDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
      //string end = endRangeDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
      string sql2 = $@"
      select *
      from c 
      where
        c.entityType = 'AvailPeriod' and c.userId = @userId
        and (

            -- periods that are active within the range
            (c.startDate >= @start and c.endDate <= @end)

            -- periods that are active
            or (@start between c.startDate and c.endDate)
            
            -- periods that become active during range, and end after
            or (@end between c.startDate and c.endDate)
        )
      ";
      var paramaters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId),
        new KeyValuePair<string, string>("@start", start),
        new KeyValuePair<string, string>("@end", end)
      };
      return await _db.GetMultipeAsync<AvailabilityPeriod>(sql2, paramaters);
    }
    /// <summary>
    /// Get a properties of a given period.
    /// </summary>
    /// <param name="userId">Service provider user id.</param>
    /// <param name="periodId">Id of period to retrieve all properties for.</param>
    /// <returns>Availability period with all properties.</returns>
    public async Task<AvailabilityPeriod> GetPeriodAsync(string userId, string periodId)
    {
      string sql = $@"
      SELECT * 
      FROM c
      where 
          c.entityType = 'AvailPeriod'
          and c.userId = @userId
          and c.id = @periodId
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId),
        new KeyValuePair<string, string>("@periodId", periodId)
      };
      return await _db.GetSingleAsync<AvailabilityPeriod>(sql, parameters);
    }
    public async Task<List<AvailabilityPeriod>> GetPeriodsAsync(string userId)
    {
      var sql = $@"
      select * from c
       where 
          c.entityType = 'AvailPeriod' 
          and c.userId = @userId
      order by c.startDate
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId)
      };
      return await _db.GetMultipeAsync<AvailabilityPeriod>(sql, parameters);
    }
    public async Task<GetPeriodDatesResponse> GetPeriodDatesAsync(string userId)
    {
      var sql = $@"
      select
        c.startDate
        , c.endDate
        , c.userId
      from c 
      where 
        c.entityType = 'AvailPeriod' 
        and c.userId = @userId
      order by c.startDate
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId)
      };
      var periodDatesView = await _db.GetMultipeAsync<PeriodDatesDocumentView>(sql, parameters);
      var response = new GetPeriodDatesResponse()
      {
        PeriodDates = new List<Models.Api.PeriodDatesApiModel>()
      };
      foreach (PeriodDatesDocumentView documentView in periodDatesView)
      {
        response.PeriodDates.Add(new Models.Api.PeriodDatesApiModel()
        {
          PeriodStart = documentView.StartDate,
          PeriodEnd = documentView.EndDate
        });
      }
      return response;
    }
    //this one just excludes a particular period from results
    // helpful when editing a period, and want the dates that were associated it with it
    // to be back in the bucket of available period dates
    public async Task<GetPeriodDatesResponse> GetPeriodDatesWithExclusionAsync(string userId, string excludePeriodId)
    {
      var sql = $@"
      select
        c.startDate
        , c.endDate
        , c.userId
      from c 
      where 
        c.entityType = 'AvailPeriod' 
        and c.userId = @userId
        and c.id != @excludePeriodId
      order by c.startDate
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId),
        new KeyValuePair<string, string>("@excludePeriodId", excludePeriodId)
      };
      var periodDatesView = await _db.GetMultipeAsync<PeriodDatesDocumentView>(sql, parameters);
      var response = new GetPeriodDatesResponse()
      {
        PeriodDates = new List<Models.Api.PeriodDatesApiModel>()
      };
      foreach (PeriodDatesDocumentView documentView in periodDatesView)
      {
        response.PeriodDates.Add(new Models.Api.PeriodDatesApiModel()
        {
          PeriodStart = documentView.StartDate,
          PeriodEnd = documentView.EndDate
        });
      }
      return response;
    }
    public async Task CreatePeriodAsync(SavePeriodRequest request)
    {
      var period = new AvailabilityPeriod()
      {
        StartDate = (DateTime)request.PeriodStart,
        EndDate = request.PeriodEnd,
        Availability = request.AvailabilityDays
      };
      period.UserId = request.ServiceProviderId;
      period.Id = Guid.NewGuid().ToString();
      await _db.CreateNoReturnAsync(period);
    }
    /// <summary>
    /// Replaces a given Availability Period document with
    /// data from request.
    /// </summary>
    /// <param name="request">Can include new Period start, end and daily availability.</param>
    /// <returns>Nothing if successful, exception otherwise.</returns>
    public async Task ReplacePeriodAsync(UpdatePeriodRequest request)
    {
      string sql = $@"
      select * 
      from c 
      where 
        c.entityType = 'AvailPeriod' 
        and c.userId = @userId
      and c.id = @periodId
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", request.ServiceProviderId),
        new KeyValuePair<string, string>("@periodId", request.PeriodId)
      };
      var periodToPatch = await _db.GetSingleAsync<AvailabilityPeriod>(sql, parameters);
      //todo: require count > 0 ?
      if (request.AvailabilityDays != null)
      {
        periodToPatch.Availability = request.AvailabilityDays;
      }
      //todo: period start is DateTime and will never be null
      if (request.PeriodStart != null && periodToPatch.StartDate != request.PeriodStart)
      {
        periodToPatch.StartDate = (DateTime)request.PeriodStart;
      }
      //end could be null
      if (request.PeriodEnd != null && request.PeriodEnd != periodToPatch.EndDate)
      {
        periodToPatch.EndDate = request.PeriodEnd;
      }
      await _db.ReplaceNoReturnAsync(periodToPatch);
    }
  }
}
