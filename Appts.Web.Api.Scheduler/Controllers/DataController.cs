using Appts.Dal.Cosmos;
using Appts.Models.Document;
using Appts.Models.Document.Stocks;
using Appts.Models.Rest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Appts.Web.Api.Scheduler.Controllers
{
  [Authorize]
  public class DataController : Controller
  {
    private readonly IDb _db;
    public DataController(IDb db)
    {
      _db = db;
    }
    [HttpPost]
    public BulkDeleteDataResponse BulkDeleteByEntityType([FromBody]BulkDeleteDataRequest request)
    {
      string deletionQuery = GetCosmosDeleteQuery(request.UserId, request.EntityType, request.DeleteAllEntities);
      var result = _db.ExecuteStoredProcAsync(deletionQuery, request.UserId).GetAwaiter().GetResult();
      return new BulkDeleteDataResponse() { Deleted = result.Deleted };
    }
    public string GetCosmosDeleteQuery(string userId, string entityType, bool deleteAllEntities)
    {
      string query = $" select * from c where c.userId = '{userId}' ";
      if (!deleteAllEntities)
      {
        query += $" and c.entityType = '{entityType}' ";
      }
      //return $" select * from c where c.userId = '{userId}' and c.entityType = '{entityType}' ";
      return query;
    }
    public List<DailyHistoricalStockPeriodDocument> GetDailyHistoricalStockPeriodDocuments(string symbol = "")
    {
      if (symbol.Length > 0)
      { 
        return _db.GetMultipeAsync<DailyHistoricalStockPeriodDocument>($"select * from c where c.entityType = 'DailyHistoricalStockPeriod' and c.symbol = '{symbol}' ").GetAwaiter().GetResult();
      
      }
      else
      {
        return _db.GetMultipeAsync<DailyHistoricalStockPeriodDocument>("select * from c where c.entityType = 'DailyHistoricalStockPeriod' ").GetAwaiter().GetResult();

      }
    }
    public List<LookupDocument> GetVanityLookups()
    {
      return _db.GetMultipeAsync<LookupDocument>("select * from c where c.entityType = 'Lookup' ").GetAwaiter().GetResult();
    }
    public ViewDocumentsByUserIdResponse GetUserDocs(string userId)
    {
      var response = new ViewDocumentsByUserIdResponse();
      var docs = _db.GetMultipeAsync<Document>($"select  * from c where c.userId = '{userId}'").GetAwaiter().GetResult();
      foreach (Document document in docs)
      {
        switch (document.EntityType)
        {
          case "Subscription":
            response.Subscription = _db.GetSingleAsync<SubscriptionDocument>($"select * from c where c.userId = '{userId}' and c.entityType = '{document.EntityType}' ").GetAwaiter().GetResult();
            break;
          case "ServiceProvider":
            response.ServiceProvider = _db.GetSingleAsync<ServiceProviderDocument>($"select * from c where c.userId = '{userId}' and c.entityType = '{document.EntityType}' ").GetAwaiter().GetResult();
            break;
          case "TrialPromo":
            response.TrialPromo = _db.GetSingleAsync<TrialPromotionDocument>($"select * from c where c.userId = '{userId}' and c.entityType = '{document.EntityType}' ").GetAwaiter().GetResult();
            break;
          case "StripeCustomer":
            response.StripeCustomer = _db.GetSingleAsync<StripeCustomerDocument>($"select * from c where c.userId = '{userId}' and c.entityType = '{document.EntityType}' ").GetAwaiter().GetResult();
            break;
          case "Client":
            response.Client = _db.GetSingleAsync<ClientDoc>($"select * from c where c.userId = '{userId}' and c.entityType = '{document.EntityType}' ").GetAwaiter().GetResult();
            break;
          case "AvailPeriod":
            response.Availability = _db.GetMultipeAsync<AvailabilityPeriod>($"select * from c where c.userId = '{userId}' and c.entityType = '{document.EntityType}' ").GetAwaiter().GetResult();
            break;
          case "ApptType":
            response.AppointmentTypes = _db.GetMultipeAsync<AppointmentType>($"select * from c where c.userId = '{userId}' and c.entityType = '{document.EntityType}' ").GetAwaiter().GetResult();
            break;
          case "Appointment":
            response.Appointments = _db.GetMultipeAsync<Appointment>($"select * from c where c.userId = '{userId}' and c.entityType = '{document.EntityType}' ").GetAwaiter().GetResult();
            break;
          default:
            break;
        }
      }
      return response;
    }
  }
}
