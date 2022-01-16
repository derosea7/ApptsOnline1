using Appts.Web.Ui.Scheduler.Services;
using Appts.Web.Ui.Scheduler.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  [Authorize]
  public class VController : Controller
  {
    private readonly IApiClient _api;
    private readonly IHttpContextResolverService _httpContext;
    public VController(IApiClient api, IHttpContextResolverService httpContext)
    {
      _api = api;
      _httpContext = httpContext;
    }
    public IActionResult Index()
    {
      return View();
    }
    [HttpGet("[controller]/[action]/{channelId}")]
    public IActionResult Channel(string channelId)
    {
      _api.ChangeBaseAddress("https://ao-video.herokuapp.com/");
      _api.RemoveAuthHeader();
      Random rand = new Random();
      string randomUserId = Convert.ToString(rand.Next());
      var vm = new VideoCallVm()
      {
        //api supports 32 bit signed int
        //UserId = _httpContext.GetUserId(),
        UserId = randomUserId,
        AppId = "7170f3361f744a148eff7d2966734c05",
        ChannelId = channelId,
        ScreenShareUid = rand.Next()
      };
      var result = _api.GetAsync<TokenResponse>($"access_token?channel={channelId}&uid={vm.UserId}").GetAwaiter().GetResult();
      vm.Token = result.Token;

      var screensharetoken = _api.GetAsync<TokenResponse>($"access_token?channel={channelId}&uid={vm.ScreenShareUid}").GetAwaiter().GetResult();
      vm.ScreenShareToken = screensharetoken.Token;
      return View(vm);
    }
  }
}
