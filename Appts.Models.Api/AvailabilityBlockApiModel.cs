using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Api
{
  public class AvailabilityBlockApiModel
  {
    [JsonProperty(PropertyName = "s")]
    public TimeSpan StartTime { get; set; }

    [JsonProperty(PropertyName = "e")]
    public TimeSpan EndTime { get; set; }
  }
}
