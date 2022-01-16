using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class HelpController : Controller
  {
    private TelemetryClient _telemetry;
    public HelpController(TelemetryClient telemetry)
    {
      _telemetry = telemetry;
    }
    public IActionResult Index()
    {
      _telemetry.TrackEvent("HelpRequested");
      _telemetry.TrackPageView("HelpHomeCenterRequested");
      return View();
    }
    public IActionResult Availability()
    {
      _telemetry.TrackEvent("AvailabilityHelpRequested");
      _telemetry.TrackPageView("AvailabilityHelpRequested");
      return View();
    }
    public IActionResult Faqs()
    {
      _telemetry.TrackEvent("FaqsRequested");
      _telemetry.TrackPageView("FaqsRequested");
      return View();
    }
}
}