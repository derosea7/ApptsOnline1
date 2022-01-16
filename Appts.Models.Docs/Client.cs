using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Docs
{
  public class Client : User
  {
    [JsonProperty("clientStuff")]
    public string ClientStuff { get; set; }
  }
}
