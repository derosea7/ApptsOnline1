using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Appts.Web.Ui.Scheduler.Models;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class HomeController : Controller
  {
    private TelemetryClient _telemetry;
    public HomeController(TelemetryClient telemetry)
    {
      _telemetry = telemetry;
    }
    public IActionResult Index()
    {
      ViewData["Landing"] = "true";
      if (User.Identity.IsAuthenticated)
      {
        RedirectToAction("Index", "Dashboard");
      }
      _telemetry.TrackPageView("HomePageRequested");
      return View();
    }
    public IActionResult About()
    {
      ViewData["Message"] = "About Appts Online";
      _telemetry.TrackEvent("AboutPageRequested");
      _telemetry.TrackPageView("AboutPageRequested");
      return View();
    }
    public IActionResult Contact()
    {
      ViewData["Message"] = "How to contact Appts Online";
      _telemetry.TrackEvent("ContactPageRequested");
      _telemetry.TrackPageView("ContactPageRequested");
      return View();
    }
    public IActionResult Privacy()
    {
      return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}