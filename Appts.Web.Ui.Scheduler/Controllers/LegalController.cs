using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class LegalController : Controller
  {
    private TelemetryClient _telemetry;
    public LegalController(TelemetryClient telemetryClient)
    {
      _telemetry = telemetryClient;
    }
    public IActionResult Terms()
    {
      _telemetry.TrackEvent("TermsOfServiceReviewed");
      _telemetry.TrackPageView("TermsOfService");
      return View();
    }
    public IActionResult Privacy()
    {
      _telemetry.TrackEvent("PrivacyPolicyReviewed");
      _telemetry.TrackPageView("PrivacyPolicyService");
      return View();
    }
    public IActionResult Dpa()
    {
      _telemetry.TrackEvent("DpaTermsReviewed");
      _telemetry.TrackPageView("DpaTermsReviewed");
      return View();
    }
  }
}