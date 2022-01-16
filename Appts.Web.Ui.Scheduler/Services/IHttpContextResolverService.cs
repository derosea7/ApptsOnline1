using Appts.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Services
{
  public interface IHttpContextResolverService
  {
    IEnumerable<Claim> GetUserClaims();
    string GetUserId();
    string GetHost();
    UserRole GetUserRole(string spId = null);
  }
}
