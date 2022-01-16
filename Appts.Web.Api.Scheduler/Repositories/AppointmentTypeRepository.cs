using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Document;
using Appts.Dal.Cosmos;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public class AppointmentTypeRepository : IAppointmentTypeRepository
  {
    private readonly IDb _db;
    public AppointmentTypeRepository(IDb db)
    {
      _db = db;
    }
    public async Task Create(AppointmentType type)
    {
      await _db.CreateNoReturnAsync(type);
    }
    public async Task Update(AppointmentType type)
    {
      await _db.ReplaceNoReturnAsync(type);
    }
    /// <summary>
    /// Soft delete the given appt type.
    /// Deleting causes issues with dependencies.
    /// </summary>
    /// <param name="apptTypeId">Id of type to mark as deleted</param>
    /// <param name="serviceProviderId">Service provider user id.</param>
    /// <returns>Void if successfully, exception otherwise.</returns>
    public async Task Delete(string apptTypeId, string serviceProviderId)
    {
      var activeDoc = await GetAppointmentTypeDetailAsync(serviceProviderId, apptTypeId);
      activeDoc.Deleted = true;
      await _db.ReplaceNoReturnAsync(activeDoc);
    }
    /// <summary>
    /// Get all properties of a given appt type.
    /// </summary>
    /// <param name="userId">Service Provider who owns the appt type.</param>
    /// <param name="apptTypeId">Appointment Type to search for</param>
    /// <returns></returns>
    public async Task<AppointmentType> GetAppointmentTypeDetailAsync(string userId, string apptTypeId)
    {
      string sql = $@"
      select * 
      from c
      where     
        c.userId = @userId
        and c.entityType = 'ApptType'
        and c.id = @apptTypeId
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId),
        new KeyValuePair<string, string>("@apptTypeId", apptTypeId)
      };
      return await _db.GetSingleAsync<AppointmentType>(sql, parameters);
    }
    /// <summary>
    /// Returns selective list of appt type properties.
    /// Can be used to select appt type or display partial info about.
    /// </summary>
    /// <param name="userId">Service Provider userId this appt type belongs to.</param>
    /// <param name="activeOnly">If true, add filter criteria to only pull active types.</param>
    /// <param name="includeDeleted">If true, add filter criteria to additionally pull soft-deleted types.</param>
    /// <returns>List of appt types with selected properties</returns>
    public async Task<List<AppointmentType>> GetAppointmentTypesAsync(
      string userId, bool activeOnly = false, bool includeDeleted = false)
    {
      string sql = $@"
      SELECT                          
        c.name                       
        , c.description              
        , c.duration
        , c.bufferBefore
        , c.bufferAfter
        , c.maximumNotice
        , c.location
        , c.locationDetails
        , c.id            
        , c.deleted
      FROM c                          
      where                           
        c.userId = '{userId}'       
        and c.entityType = 'ApptType'
      ";
      var parameters = new List<KeyValuePair<string, string>>()
      {
        new KeyValuePair<string, string>("@userId", userId),
      };
      if (activeOnly)
        sql += " and c.isActive = true";
      if (!includeDeleted)
        sql += " and c.deleted = false";
      return await _db.GetMultipeAsync<AppointmentType>(sql, parameters);
    }
  }
}
