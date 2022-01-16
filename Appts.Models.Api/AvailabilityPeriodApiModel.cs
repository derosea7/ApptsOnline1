using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Api
{
  public class AvailabilityPeriodApiModel
  {
    [JsonProperty(PropertyName = "timeZoneId")]
    public string TimeZoneId { get; set; }

    [JsonProperty(PropertyName = "startDate")]
    public DateTime StartDate { get; set; }

    [JsonProperty(PropertyName = "endDate")]
    public DateTime? EndDate { get; set; }

    // true when period encompasses today
    [JsonProperty(PropertyName = "isCurrent")]
    public bool IsCurrent { get; set; }

    // true when period encompasses today
    [JsonProperty(PropertyName = "isFuture")]
    public bool IsFuture { get; set; }

    [JsonProperty(PropertyName = "availability")]
    public List<AvailabilityApiModel> Availability { get; set; }
  }
}
