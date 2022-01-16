using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Api
{
  public class AvailabilityApiModel
  {
    /// <summary>
    /// Only used when re-displaying back to client with
    /// concrete dates. When the Availability is used
    /// as a generic day, this will be null.
    /// </summary>
    [JsonProperty(PropertyName = "concreteDate")]
    public DateTime ConcreteDate { get; set; }

    [JsonProperty(PropertyName = "blocks")]
    public List<AvailabilityBlockApiModel> Blocks { get; set; }
  }
}
