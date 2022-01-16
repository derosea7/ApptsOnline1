using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Common.Constants;
using Appts.Models.Document;
using Appts.Web.Ui.Scheduler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Appts.Models.Rest;
using Microsoft.Extensions.Logging;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  /// <summary>
  /// Support connected social media calendars, primarily Google Calendar.
  /// </summary>
  [Authorize("PaidSubscriber")]
  public class CalendarsController : Controller
  {
    private readonly IApiClient _apiClient;
    private readonly IGoogleCalendarClient _googleCalendarClient;
    private readonly ILogger<CalendarsController> _logger;
    public CalendarsController(
      IApiClient apiClient, 
      IGoogleCalendarClient googleCalendarClient,
      ILogger<CalendarsController> logger)
    {
      _apiClient = apiClient;
      _googleCalendarClient = googleCalendarClient;
      _logger = logger;
    }
    /// <summary>
    /// Show service provider a list of their connected calendar.
    /// First calendar possible will be gcal.
    /// </summary>
    /// <returns>Model with list of calendars associated with Appts.Online</returns>
    public IActionResult Index()
    {
      string userId = User.Claims.FirstOrDefault(c => c.Type == IdentityK.NameIdentifier).Value;
      string endpoint = $"/api/Calendars/GetActiveCalendars/{userId}";
      List<Calendar> calendars = _apiClient.GetAsync<List<Calendar>>(endpoint)
        .GetAwaiter().GetResult();
      return View(calendars);
    }
    // connect a new google calender
    public IActionResult ConnectGoogleCalendar()
    {
      string spUserId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      _googleCalendarClient.Initialize(spUserId);
      // check if user wants to add another calendar if they have one?
      return Redirect(_googleCalendarClient.GetAuthorizationUrl());
    }
    public IActionResult DisconnectGoogleCalendar()
    {
      bool revokedGcal = false;
      string spUserId = User.Claims.First(e => e.Type == IdentityK.NameIdentifier).Value;
      try
      {
        _googleCalendarClient.Initialize(spUserId);
        _googleCalendarClient.RevokeCalendarApiAccessForUserAsync().GetAwaiter().GetResult();
        revokedGcal = true;
      }
      catch (Exception ex)
      {
        _logger.LogError("Failed to revoke GCal via Google Api", ex.Message, ex.StackTrace);
      }
      var request = new DisconnectConnectedCalendarRequest()
      {
        UserId = spUserId
      };
      var response = _apiClient.PostAsync<DisconnectConnectedCalendarRequest, DisconnectConnectedCalendarResponse>(
        request, "api/Calendars/DisconnectCalendar").GetAwaiter().GetResult();
      TempData["CalendarDisconnected"] = revokedGcal.ToString();
      return RedirectToAction("Index");
    }
  }
}