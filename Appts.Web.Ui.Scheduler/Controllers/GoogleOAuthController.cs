using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Common.Constants;
using Appts.Web.Ui.Scheduler.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Appts.Web.Ui.Scheduler.Controllers
{
  /// <summary>
  /// Sole purpose is to receive Authorization Code from Google OAuth
  /// flow. Best-practice to not redirect to page with more-than-neccessary
  /// client-side scripts, as authorization code comes in as query string param.
  /// </summary>
  public class GoogleOAuthController : Controller
  {
    private readonly IGoogleCalendarClient _googleCalendarClient;
    public GoogleOAuthController(IGoogleCalendarClient googleCalendarClient)
    {
      _googleCalendarClient = googleCalendarClient;
    }

    /// <summary>
    /// This endpoint is hit after user authorizes and gives consent.
    /// 
    /// We retreive the authorization code and exchange it for 
    /// access and refresh tokens.
    /// 
    /// Side effects:
    ///   - Access token is cached in Redis
    ///   - Access and refresh token is saved in Cosmos
    /// </summary>
    /// <param name="code">Authorization code from google for user</param>
    /// <returns>Redirects user to view their newly connected calendar</returns>
    public IActionResult AuthorizationCode([FromQuery] string code)
    {
      if (string.IsNullOrEmpty(code))
      {
        /* 
            This means the user canceled and did not grant us access. In this case, there will be a query parameter
            on the request URL called 'error' that will have the error message. You can handle this case however.
            Here, we'll just not do anything, but you should write code to handle this case however your application
            needs to.
        */
      }
      //_googleCalendarClient.UserId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      string spUserId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      _googleCalendarClient.Initialize(spUserId);
      var token = _googleCalendarClient.ExchangeCodeForTokenAsync(code)
        .GetAwaiter().GetResult();

      return RedirectToAction("Index", "Calendars");
    }
  }
}
