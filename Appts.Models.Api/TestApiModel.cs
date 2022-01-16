using System;
using Newtonsoft.Json;

namespace Appts.Models.Api
{
  public class TestApiModel
  {
    [JsonProperty(PropertyName = "testVale")]
    public string TestValue { get; set; }
  }
}
