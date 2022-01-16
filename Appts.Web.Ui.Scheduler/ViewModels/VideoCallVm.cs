using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class VideoCallVm
  {
    public string Token { get; set; }
    public string AppId { get; set; }
    public string UserId { get; set; }
    public string ChannelId { get; set; }

    public string ScreenShareToken { get; set; }
    public int ScreenShareUid { get; set; }
  }
}
