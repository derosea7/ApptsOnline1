using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class GetAppointmentDetailRequest
  {
    //wont use since annon clients wont have user id, only spvanityurl
    [JsonProperty(PropertyName = "userId")]
    public string UserId { get; set; }

    [JsonProperty(PropertyName = "spVanityUrl")]
    public string SpVanityUrl { get; set; }

    [JsonProperty(PropertyName = "apptId")]
    public string ApptId { get; set; }
  }
}
