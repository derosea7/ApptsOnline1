using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class Email : Document
  {
    public Email()
    {
      EntityType = "Email";
    }
    [JsonProperty(PropertyName = "body")]
    public string Body { get; set; }
  }
}
