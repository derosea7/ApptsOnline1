using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class SmsController : Controller
  {
    public IActionResult Index()
    {
      //var msg = 
      //string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
      //string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
      //string accountSid = "AC38857db0f3c302b892e017dc0c3405b8";
      //string authToken = "c5d06042078d3a78aaa1c8b587666d13";
      //TwilioClient.Init(accountSid, authToken);

      //var message = MessageResource.Create(
      //    body: "Hello world.",
      //    from: new Twilio.Types.PhoneNumber("+12057497723"),
      //    to: new Twilio.Types.PhoneNumber("+15204944767")
      //);

      return View();
    }
  }
}
