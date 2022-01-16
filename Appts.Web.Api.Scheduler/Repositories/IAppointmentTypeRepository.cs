using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Api.Scheduler.Repositories
{
  public interface IAppointmentTypeRepository
  {
    /// <summary>
    /// Returns selective list of appt type properties.
    /// Can be used to select appt type or display partial info about.
    /// </summary>
    /// <param name="userId">Service Provider userId this appt type belongs to.</param>
    /// <param name="activeOnly">If true, add filter criteria to only pull active types.</param>
    /// <param name="includeDeleted">If true, add filter criteria to additionally pull soft-deleted types.</param>
    /// <returns>List of appt types with selected properties</returns>
    Task<List<AppointmentType>> GetAppointmentTypesAsync(
      string userId, bool activeOnly = false, bool includeDeleted = false);
    Task Create(AppointmentType type);
    Task Update(AppointmentType type);
    Task Delete(string apptTypeId, string serviceProviderId);
    /// <summary>
    /// Get all properties of a given appt type.
    /// </summary>
    /// <param name="userId">Service Provider who owns the appt type.</param>
    /// <param name="apptTypeId">Appointment Type to search for</param>
    /// <returns></returns>
    Task<AppointmentType> GetAppointmentTypeDetailAsync(string userId, string apptTypeId);
  }
}
