using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Common.Constants;
using Appts.Models.Document;
using Appts.Models.Domain;
using Appts.Models.Rest;
using Appts.Models.View;
using Appts.Web.Ui.Scheduler.Models;
using Appts.Web.Ui.Scheduler.Repositories;
using Appts.Web.Ui.Scheduler.Services;
using Appts.Web.Ui.Scheduler.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  [Authorize]
  public class DashboardController : Controller
  {
    private readonly IApiClient _apiClient;
    private readonly IAppointmentRepository _apptRepo;
    private readonly IHttpContextResolverService _httpContext;
    private TelemetryClient _telemetry;
    public DashboardController(IApiClient apiClient, 
      IAppointmentRepository appointmentRepository,
      IHttpContextResolverService httpContextResolverService,
      TelemetryClient telemetry)
    {
      _apiClient = apiClient;
      _apptRepo = appointmentRepository;
      _httpContext = httpContextResolverService;
      _telemetry = telemetry;
    }
    // GET: /<controller>/
    [HttpGet]
    public IActionResult Index()
    {
      _telemetry.TrackEvent("DashboardRequested");
      string  userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      UserRole role = _httpContext.GetUserRole();
      var request = new ViewAppointmentsRequest()
      {
        UserId = userId
      };
      ApptsCalendarViewModel vm = new ApptsCalendarViewModel()
      {
        UserRole = role
      };
      return View(vm);
    }
    [HttpPost]
    [ActionName("Index")]
    public IActionResult IndexPost()
    {
      _telemetry.TrackEvent("DashboardPostRequested");
      _telemetry.TrackPageView("DashboardPostRequested");
      var request = new GetSubscriptionPlanRequest()
      {
        ObjectId = "adam"
      };
      string endpoint = $"/api/Subscription/GetActivePlan";
      var response = new GetSubscriptionPlanResponse();
      try
      {
        response = _apiClient.PostAsync<GetSubscriptionPlanRequest, GetSubscriptionPlanResponse>(
          request, endpoint
        ).GetAwaiter().GetResult();
      }
      catch (Exception ex)
      {
        response.Ext_orgId = "Error";
      }
      if (response.Ext_orgId == null)
        response.Ext_orgId = "";
      return View();
    }
  }
}