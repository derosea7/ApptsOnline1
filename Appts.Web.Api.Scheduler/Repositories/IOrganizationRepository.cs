using Appts.Models.Api;
using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Api.Scheduler.Repositories
{
  public interface IOrganizationRepository
  {
    Task<TestApiModel> GetTestAsync();

    Task<string> GetOrganizationIdenity(string memberId);
  }
}
