using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Document;
namespace Appts.Models.Rest
{
  public class GetCalendarEventsResponse
  {
    //Calendar events = 
    public List<CalendarEvent> CalendarEvents { get; set; }
  }
}
