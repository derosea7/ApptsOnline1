using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Appts.Web.Ui.Scheduler.ViewModels;
using Appts.Web.Ui.Scheduler.Services;
using Appts.Models.Rest;
using Appts.Models.Document;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  [Authorize]
  public class VideoController : Controller
  {
    private readonly IApiClient _api;
    private readonly IHttpContextResolverService _httpContext;
    public VideoController(IApiClient api, IHttpContextResolverService httpContext)
    {
      _api = api;
      _httpContext = httpContext;
    }
    //[HttpGet("[controller]")]
    public IActionResult Index()
    {
      //_api.ChangeBaseAddress("https://api-scheduler.appts.online/");
      _api.ChangeBaseAddress("https://ao-video.herokuapp.com/");
      _api.RemoveAuthHeader();
      Random rand = new Random();
      string randomUserId = Convert.ToString(rand.Next());
      var vm = new VideoCallVm()
      {
        //UserId = _httpContext.GetUserId(),
        UserId = randomUserId,
        AppId = "7170f3361f744a148eff7d2966734c05"
      };
      var result = _api.GetAsync<TokenResponse>($"access_token?channel=ao4&uid={vm.UserId}").GetAwaiter().GetResult();
      vm.Token = result.Token;
      return View(vm);
    }
    public IActionResult Meeting()
    {
      var result = _api.GetAsync<List<VideoMeetingDoc>>($"api/Video/Meetings/{_httpContext.GetUserId()}").GetAwaiter().GetResult();
      var vm = new ViewVideoMeetingsVm()
      { 
        VideoMeetings = result
      };
      return View("VideoMeeting", vm);
    }
    //a video channel
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
        ChannelId = channelId
      };
      var result = _api.GetAsync<TokenResponse>($"access_token?channel={channelId}&uid={vm.UserId}").GetAwaiter().GetResult();
      vm.Token = result.Token;
      return View(vm);
    }
    [HttpGet("[controller]/[action]")]
    public IActionResult NewMeeting()
    {
      string channelId = Guid.NewGuid().ToString();
      return View();
    }
    [HttpPost("[controller]/[action]")]
    [ActionName("NewMeeting")]
    public IActionResult NewMeetingPost([FromForm]NewVideoMeetingVm model)
    {
      model.ChannelId = Guid.NewGuid().ToString();
      var request = new SaveNewVideoMeetingRequest()
      { 
        ChannelId = model.ChannelId,
        HostUserId = _httpContext.GetUserId(),
        ChannelName = model.ChannelName
      };
      string endpoint = "api/Video/NewMeeting";
      _api.PostNoReturnAsync<SaveNewVideoMeetingRequest>(request, endpoint).GetAwaiter().GetResult();
      //save channel to cosmos here
      //with returned channel doucment response, forward user to channel url
      //return RedirectToAction($"Channel%2F{model.ChannelId}", "Video");
      return RedirectToAction($"Channel", "Video", new { id = model.ChannelId });
    }
  }
  internal class TokenResponse
  {
    [JsonProperty("token")]
    public string Token { get; set; }
  }
}