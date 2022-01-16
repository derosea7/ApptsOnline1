using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document;
using Appts.Web.Api.Scheduler.Repositories;
using Microsoft.AspNetCore.Mvc;
using Appts.Models.Rest;

namespace Appts.Web.Api.Scheduler.Controllers
{
  //[Route("api/[controller]/[action]")]
  public class AppointmentTypeController : Controller
  {
    private readonly IAppointmentTypeRepository _appointmentTypeRepository;
    public AppointmentTypeController(
      IAppointmentTypeRepository appointmentTypeRepository)
    {
      _appointmentTypeRepository = appointmentTypeRepository;
    }
    [HttpPost("api/[controller]/[action]")]
    public void Create([FromBody] AppointmentType appointmentType)
    {
      _appointmentTypeRepository.Create(appointmentType)
        .GetAwaiter().GetResult();
    }
    [HttpPatch("api/[controller]/[action]")]
    public void Update([FromBody] AppointmentType appointmentType)
    {
      _appointmentTypeRepository.Update(appointmentType)
        .GetAwaiter().GetResult();
    }
    [HttpPost("api/[controller]/[action]")]
    public void Delete([FromBody]DeleteAppointmentTypeRequest request)
    {
      _appointmentTypeRepository.Delete(
        request.AppointmentTypeId, request.ServiceProviderId)
        .GetAwaiter().GetResult();
    }
    /// <summary>
    /// Returns list of appointment types.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("api/[controller]/[action]")]
    public List<AppointmentType> GetAppointmentTypes(string userId, bool activeOnly = false)
    {
      return _appointmentTypeRepository.GetAppointmentTypesAsync(userId, activeOnly)
        .GetAwaiter().GetResult();
    }
    [HttpGet("api/[controller]/[action]/{id}")]
    public AppointmentType Detail2(string id, string userId)
    {
      return _appointmentTypeRepository.GetAppointmentTypeDetailAsync(userId, id)
        .GetAwaiter().GetResult();
    }
  }
}
