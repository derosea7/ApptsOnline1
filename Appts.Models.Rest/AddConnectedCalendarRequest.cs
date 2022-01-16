using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class AddConnectedCalendarRequest
  {
    public Calendar CalendarToAdd { get; set; }
    public TokenCache TokenCache { get; set; }
    public string UserId { get; set; }
  }
}
