using Appts.Common.Constants;
using Appts.Models.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Services
{
  public class HttpContextResolverService : IHttpContextResolverService
  {
    private readonly IHttpContextAccessor _context;

    public HttpContextResolverService(IHttpContextAccessor context)
    {
      _context = context;
    }

    //public bool UserHasClaims()
    //{

    //}
    public UserRole GetUserRole(string spId = null)
    {
      
      //string subStat = claims.FirstOrDefault(c => c.Type == IdentityK.SubStat).Value;
      UserRole userRole = UserRole.AnonymousScheduler;
      if (!_context.HttpContext.User.Identity.IsAuthenticated)
      {
        userRole = UserRole.AnonymousScheduler;
      }
      else
      {
        //type could be client, sp, or admin. 
        //white list prevents elevated privledges for client or sp.
        string[] whitelistTypeValues = new string[] { "Client", "Sp" };
        string type = _context.HttpContext.User.Claims
          .Where(c => whitelistTypeValues.Contains(c.Value))
          .FirstOrDefault(c => c.Type == IdentityK.Type).Value;
        if (type == "Sp")
        {
          userRole = UserRole.Subscriber;
        }
        else if (type == "Client")
        {
          userRole = UserRole.Client;
        }
      }
      return userRole;
    }
    public IEnumerable<Claim> GetUserClaims()
    {
      return _context.HttpContext.User?.Claims;
    }
    public string GetUserId()
    {


      try
      {
        bool s = _context.HttpContext.User.Identity.IsAuthenticated;
      }
      catch (Exception ex)
      {

        throw;
      }

      string result = "";
      if (_context.HttpContext.User.Identity.IsAuthenticated == false)
      {

      }
      else
      {
        result = _context.HttpContext.User?.Claims?
        .FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      }

      return result;
    }

    public string GetHost()
    {
      return _context.HttpContext.Request.Host.ToString();
    }
  }
}
