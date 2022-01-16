using Appts.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document;
using Appts.Models.Domain;

namespace Appts.Web.Api.Scheduler.Repositories
{
  public interface IAvailabilityRepository
  {
    Task<GetAvailabilityForSchedulingResponse> GetAvailabilityForSchedulingAsync(
      GetAvailabilityForSchedulingRequest request);
    Task<AvailabilityPeriod> GetPeriodAsync(string userId, string periodId);
    Task<List<AvailabilityPeriod>> GetPeriodsAsync(string userId);
    Task<GetPeriodDatesResponse> GetPeriodDatesAsync(string userId);
    Task<GetPeriodDatesResponse> GetPeriodDatesWithExclusionAsync(string userId, string excludePeriodId);
    Task CreatePeriodAsync(SavePeriodRequest request);
    Task DeletePeriodAsync(string userId, string periodId);
    Task ReplacePeriodAsync(UpdatePeriodRequest request);
    //Task<InitialSchedulingInfoResponse> GetInitialSchedulingInfoAsync(
    //  string serviceProviderVanityUrl, UserRole userRole, SchedulerRole schedulerRole,
    //  string clientUserId = null);
    Task<InitialSchedulingInfoResponse> GetInitialSchedulingInfoAsync(
          //string serviceProviderVanityUrl, UserRole userRole, SchedulerRole schedulerRole, 
          //string clientUserId = null)
          InitialSchedulingInfoRequest request);
    Task<InitialReschedulingInfoResponse> InitialReschedulingInfoAsync(
      InitialReschedulingInfoRequest request);
  }
}
