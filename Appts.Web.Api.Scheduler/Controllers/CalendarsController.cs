using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Dal.Cosmos;
using Appts.Models.Document;
using Appts.Models.Rest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Appts.Web.Api.Scheduler.Controllers
{
  [Authorize]
  public class CalendarsController : Controller
  {
    private readonly IDb _Db;
    public CalendarsController(IDb db)
    {
      _Db = db;
    }
    [HttpPost("api/[controller]/[action]")]
    public void AddConnectedCalendar([FromBody] AddConnectedCalendarRequest request)
    {
      _Db.CreateNoReturnAsync(request.CalendarToAdd).GetAwaiter().GetResult();
      _Db.CreateNoReturnAsync(request.TokenCache).GetAwaiter().GetResult();
    }
    [HttpPost("api/[controller]/[action]")]
    public DisconnectConnectedCalendarResponse DisconnectCalendar(
      [FromBody] DisconnectConnectedCalendarRequest request)
    {
      var response = new DisconnectConnectedCalendarResponse()
      {
        DeletedTokenCache = true,
        InactivatedCalendar = true,
        ExceptionMessages = new List<string>()
      };
      string sql = $@"
      select * from c
      where 
        c.entityType = 'Calendar'
        and c.userId = '{request.UserId}'
        and c.provider = 'Google'
        and c.active = true
        and IS_NULL(c.revokedOn) = true
      ";
      try
      {
        var original = _Db.GetSingleAsync<Calendar>(sql).GetAwaiter().GetResult();
        original.Active = false;
        original.RevokedOn = DateTime.Now;
        _Db.ReplaceAsync<Calendar>(original);
      }
      catch (Exception ex)
      {
        response.InactivatedCalendar = false;
        response.ExceptionMessages.Add(ex.Message);
      }
      string sqltokencache = $@"
      select c.id from c
      where c.userId = '{request.UserId}'
      and c.entityType = 'TokenCache'
      ";
      try
      {
        TokenCache tokenCache = _Db.GetSingleAsync<TokenCache>(sqltokencache).GetAwaiter().GetResult();
        _Db.DeleteNoReturnAsync<TokenCache>(tokenCache.Id, request.UserId).GetAwaiter().GetResult();
      }
      catch (Exception ex)
      {
        response.InactivatedCalendar = false;
        response.ExceptionMessages.Add(ex.Message);
      }
      return response;
    }
    /// <summary>
    /// Given a service provider, get a list of their actively connected calendars.
    /// </summary>
    /// <param name="spId">Service provider user id</param>
    /// <returns>List of active connected calendars for the specified provider.</returns>
    [HttpGet("api/[controller]/[action]/{spId}")]
    public List<Calendar> GetActiveCalendars(string spId)
    {
      var sql = $@"
      SELECT  c.provider, c.addedOn
      FROM c
      where
          c.entityType = 'Calendar'
          and c.provider = 'Google'
          and c.active = true
          and c.userId = '{spId}'
          and c.revokedOn = null
      ";
      return _Db.GetMultipeAsync<Calendar>(sql).GetAwaiter().GetResult();
    }
    [HttpGet("api/[controller]/[action]/{serviceProviderId}")]
    public bool HasGoogleCalendar(string serviceProviderId)
    {
      var sql = $@"
      SELECT value(count(1))
      FROM c
      where
          c.entityType = 'Calendar'
          and c.provider = 'Google'
          and c.userId = '{serviceProviderId}'
      ";
      int calendarCount = _Db.GetSingleAsync<int>(sql).GetAwaiter().GetResult();
      bool result = calendarCount > 0 ? true : false;

      return result;
    }
  }
}
