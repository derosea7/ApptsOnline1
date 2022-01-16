using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Api.Scheduler.Controllers
{
  public class SmsController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }
  }
}
