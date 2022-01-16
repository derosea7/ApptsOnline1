using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Rest;
using Appts.Dal.Cosmos;
using Appts.Models.Document;
using Microsoft.AspNetCore.Authorization;

namespace Appts.Web.Api.Scheduler.Controllers
{
  [Authorize]
  //[Route("api/[controller]/[action]")]
  public class VideoController : Controller
  {
    private readonly IDb _db;
    public VideoController(IDb db)
    {
      _db = db;
    }
    //[HttpPost("[controller]/[action]")]
    //public IActionResult NewMeeting()
    [HttpPost]  
    [ActionName("NewMeeting")]
    public void NewMeetingPost([FromBody]SaveNewVideoMeetingRequest model)
    {
      // should do some dupe checking on name here
      // so user doesnt create 2 meetings with same name
      // will be ok since channel is a guid, but still
      var doc = new VideoMeetingDoc()
      {
        ChannelId = model.ChannelId,
        ChannelName = model.ChannelName,
        HostUserId = model.HostUserId,
        Id = Guid.NewGuid().ToString(),
        UserId = model.HostUserId
      };
      _db.CreateNoReturnAsync(doc).GetAwaiter().GetResult();
    }
    public VideoMeetingDoc GetMeeting(string id, string userId)
    {
      string sql = $@"
      SELECT * FROM c
      where c.entityType = 'VideoMeeting'
      and c.userId = '{userId}'
      and c.id = '{id}'
      ";
      return _db.GetSingleAsync<VideoMeetingDoc>(sql).GetAwaiter().GetResult();
    }
    public List<VideoMeetingDoc> Meetings(string id)
    {
      string sql = $@"
      SELECT * FROM c
      where c.entityType = 'VideoMeeting'
      and c.userId = '{id}'
      ";
      return _db.GetMultipeAsync<VideoMeetingDoc>(sql).GetAwaiter().GetResult();
    }
  }
}
