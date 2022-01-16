using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class NewVideoMeetingVm
  {
    //required on post
    // channel name, stirng < 32 chars

    public string ChannelId { get; set; }
    [StringLength(32)]
    public string ChannelName { get; set; }
    public string UserId { get; set; }
    public int StreamUid { get; set; }
    public DateTime EndNoLaterThan { get; set; }
  }
}
