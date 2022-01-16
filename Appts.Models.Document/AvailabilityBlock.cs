using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class AvailabilityBlock
  {
    [JsonProperty(PropertyName = "s")]
    public TimeSpan StartTime { get; set; }

    [JsonProperty(PropertyName = "e")]
    public TimeSpan EndTime { get; set; }

    public AvailabilityBlock()
    {

    }

    public AvailabilityBlock(TimeSpan startTime, TimeSpan endTime)
    {
      StartTime = startTime;
      EndTime = endTime;
    }

    public override string ToString()
    {
      return $"{StartTime.ToString("t")} to {EndTime.ToString("t")}";
    }
  }
}
