using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Appts.Messaging.ServiceBus;
using Appts.Models.View;
using Appts.Models.Domain;
using Newtonsoft.Json;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class ReminderController : Controller
  {
    private readonly IBus _bus;
    public ReminderController(IBus bus)
    {
      _bus = bus;
    }
    [HttpPost("[controller]/[action]")]
    public IActionResult ScheduleEmailReminderTest(SendTestEmailReminderViewModel model)
    {
      var s = ModelState.IsValid;
      var email = new OutgoingEmail()
      {
        To = model.Email,
        From = "reminders@appts.online",
        Subject = model.Subject ?? "Test - Email Reminder",
        Body = model.Body
      };
      string messageBody = JsonConvert.SerializeObject(email);
      DateTime sendOn = DateTime.UtcNow.AddSeconds(model.SecondsToWait ?? 30);

      long seqNumber = _bus.ScheduleCancelableMessageAsync(messageBody, sendOn)
        .GetAwaiter().GetResult();

      return RedirectToAction("Dashboard", "User", new { es = "t", seqNum = seqNumber });
    }

    // GET: /<controller>/
    public IActionResult Index()
    {
      return View();
    }
  }
}
