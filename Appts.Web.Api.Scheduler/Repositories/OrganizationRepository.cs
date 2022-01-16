using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appts.Dal.Cosmos;
using Appts.Models.Api;
using Appts.Models.Document.Views;
namespace Appts.Web.Api.Scheduler.Repositories
{
  public class OrganizationRepository : IOrganizationRepository
  {
    private readonly IDb _db;
    private readonly ISubscriptionRepository _subscriptionRepository;
    public OrganizationRepository(
      IDb db,
      ISubscriptionRepository subscriptionRepository)
    {
      _db = db;
      _subscriptionRepository = subscriptionRepository;
    }
    public async Task<TestApiModel> GetTestAsync()
    {
      string sql = "select c.testValue from c where c.entityType = 'Test'";
      return await _db.GetSingleAsync<TestApiModel>(sql);
    }
    //11/7/19, just return either Paid, TrialExpired or nothing
    //11/7/19, moved code to subscription controller
    public async Task<string> GetOrganizationIdenity(string memberId)
    {
      //string sql = $@"
      //select c.orgId, c.roles
      //FROM c
      //where
      //c.entityType = 'OrgId'
      //and c.userId = '{memberId}'
      //";
      //OrganizationIdentityView organizationIdentity = await _db.GetSingleAsync<OrganizationIdentityView>(sql);

      //// join roles as csv
      //string id = organizationIdentity.OrganizationId;
      //foreach (string role in organizationIdentity.Roles)
      //{
      //  id += $",{role}";
      //}
      string result = "";
      string plan = await _subscriptionRepository.GetActivePlanAsync(memberId);
      if (plan != null && (plan == "FreeTrial" || plan == "Paid"))
      {
        result = "Paid";
      }
      else if (plan == null)
      {
        string sql = $@"
        SELECT value c.id 
        FROM c
        where 
            c.userId = 'adamd'
            and c.entityType = 'AfterTrialPromo'
        ";
        string promoId = await _db.GetSingleAsync<string>(sql);
        if (promoId != null)
        {
          result = "TrialPromo";
        }
      }
      return result;
    }
  }
}