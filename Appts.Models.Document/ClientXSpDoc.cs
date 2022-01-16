using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class ClientXSpDoc : Document
  {
    [JsonProperty(PropertyName = "serviceProviderId")]
    public string ServiceProviderId { get; set; }
    public ClientXSpDoc()
    {
      EntityType = "ClientXSp";
    }
  }
}
