using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  [Authorize]
  public class SiteController : Controller
  {
    public IActionResult Preferences()
    {
      return View();
    }
    [HttpPost]
    public IActionResult SetTheme(string data)
    {
      CookieOptions cookies = new CookieOptions()
      {
        Expires = DateTime.Now.AddDays(30)
      };
      Response.Cookies.Append("theme", data, cookies);
      return Ok();
    }
  }
}
