using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Rest;
using Appts.Web.Api.Scheduler.Repositories;
using Microsoft.AspNetCore.Mvc;
using Appts.Models.Document;
using Appts.Models.Domain;

namespace Appts.Web.Api.Scheduler.Controllers
{
  public class AvailabilityController : Controller
  {
    private readonly IAvailabilityRepository _availabilityRepository;
    public AvailabilityController(IAvailabilityRepository availabilityRepository)
    {
      _availabilityRepository = availabilityRepository;
    }
    [HttpPost]
    //[Route("api/[controller]/[action]/{serviceProviderVanityUrl}")]
    [Route("api/[controller]/[action]")]
    public InitialSchedulingInfoResponse InitialSchedulingInfo(
      [FromBody]InitialSchedulingInfoRequest request)
    {
      return _availabilityRepository.GetInitialSchedulingInfoAsync(request)
        .GetAwaiter().GetResult();
    }
    [HttpGet("api/[controller]/[action]")]
    public InitialReschedulingInfoResponse InitialReschedulingInfo(
      [FromQuery]InitialReschedulingInfoRequest request)
    {
      return _availabilityRepository.InitialReschedulingInfoAsync(request)
        .GetAwaiter().GetResult();
    }
    [HttpGet]
    [Route("api/[controller]/[action]")]
    public GetAvailabilityForSchedulingResponse GetAvailabilityForScheduling(
      [FromQuery]GetAvailabilityForSchedulingRequest request)
    {
      return _availabilityRepository.GetAvailabilityForSchedulingAsync(request)
        .GetAwaiter().GetResult();
    }
    [Route("api/[controller]/[action]")]
    [HttpPost]
    public void DeletePeriod([FromBody]DeleteAvailabilityPeriodRequest request)
    {
      _availabilityRepository.DeletePeriodAsync(request.UserId, request.PeriodId)
        .GetAwaiter().GetResult();
    }
    [HttpPost]
    [Route("api/[controller]/[action]")]
    public void SavePeriod([FromBody]SavePeriodRequest request)
    {
      _availabilityRepository.CreatePeriodAsync(request)
        .GetAwaiter().GetResult();
    }
    [HttpGet("api/[controller]/[action]")]
    public AvailabilityPeriod GetPeriod(string userId, string periodId)
    {
      return _availabilityRepository.GetPeriodAsync(userId, periodId)
        .GetAwaiter().GetResult();
    }
    [Route("api/[controller]/[action]/{userId}")]
    public List<AvailabilityPeriod> GetPeriods(string userId)
    {
      return _availabilityRepository.GetPeriodsAsync(userId)
        .GetAwaiter().GetResult();
    }
    [Route("api/[controller]/[action]/{userId}")]
    public GetPeriodDatesResponse GetPeriodDates(string userId)
    {
      return _availabilityRepository.GetPeriodDatesAsync(userId)
        .GetAwaiter().GetResult();
    }
    //this version excludes a period so that its dates are in results
    [Route("api/[controller]/[action]/{userId}")]
    public GetPeriodDatesResponse GetPeriodDatesWithExclusion(string userId, string excludePeriodId)
    {
      return _availabilityRepository.GetPeriodDatesWithExclusionAsync(userId, excludePeriodId)
        .GetAwaiter().GetResult();
    }
    [HttpPatch("api/[controller]/[action]")]
    public void UpdatePeriod([FromBody]UpdatePeriodRequest request)
    {
      _availabilityRepository.ReplacePeriodAsync(request)
        .GetAwaiter().GetResult();
    }
  }
}