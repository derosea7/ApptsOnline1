using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class GuideController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
    public IActionResult Goal()
    {
      return View();
    }
  }
}
