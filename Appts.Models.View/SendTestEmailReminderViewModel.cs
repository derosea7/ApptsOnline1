using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Appts.Models.View
{
  public class SendTestEmailReminderViewModel
  {
    [Required]
    public string Email { get; set; }

    public string Subject { get; set; }
    public string Body { get; set; }
    public int? SecondsToWait { get; set; }

    public long? SequenceNumberToCancel { get; set; }
  }
}
