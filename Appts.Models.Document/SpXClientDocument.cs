using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class SpXClientDocument : Document
  {
    //id of the client whom is assosciated with the service provider who has the userid of the document
    [JsonProperty(PropertyName = "clientId")]
    public string ClientId { get; set; }
    public SpXClientDocument()
    {
      EntityType = "SpXClient";
    }
  }
}
