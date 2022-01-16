using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class DisconnectConnectedCalendarResponse
  {
    public bool InactivatedCalendar { get; set; }
    public bool DeletedTokenCache { get; set; }
    public List<string> ExceptionMessages { get; set; }
  }
}
