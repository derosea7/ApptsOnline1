using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Document
{
  public class UserImage : Document
  {
    [JsonProperty("title")]
    public string Title { get; set; }

    // url where this picture lives
    [JsonProperty("url")]
    public string Url { get; set; }
  }
}
