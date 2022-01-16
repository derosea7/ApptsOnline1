using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class VideoMeetingDoc : Document
  {
    [JsonProperty("hostUserId")]
    public string HostUserId { get; set; }
    [JsonProperty("channelId")]
    public string ChannelId { get; set; }
    [JsonProperty("channelName")]
    public string ChannelName { get; set; }
    public VideoMeetingDoc()
    {
      EntityType = "VideoMeeting";
    }
  }
}
