using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Document
{
  public class CalendarEvent : Document
  {
    [JsonProperty("title")]
    public string Description { get; set; }
    [JsonProperty("start")]
    public DateTime StartDate { get; set; }
    [JsonProperty("end")]
    public DateTime EndDate { get; set; }
    public CalendarEvent()
    {
      EntityType = "CalendarEvent";
    }
  }
}
