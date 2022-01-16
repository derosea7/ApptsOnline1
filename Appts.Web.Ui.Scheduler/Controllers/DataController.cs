using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class DataController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
    public IActionResult Manage()
    {
      return View();
    }
    [HttpPost("[controller]/[action]")]
    public IActionResult DeleteApptsScheduledWithMe()
    {
      return RedirectToAction("Manage", "Data", new { success = "t" });
    }
  }
}
