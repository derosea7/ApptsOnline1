using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class SaveNewVideoMeetingRequest
  {
    public string HostUserId { get; set; }
    public string ChannelId { get; set; }
    public string ChannelName { get; set; }
  }
}
