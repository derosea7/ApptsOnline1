using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Appts.Models.Document
{
  public class TestDocument : Document
  {
    public TestDocument()
    {
      EntityType = "Test";
    }
    [JsonProperty(PropertyName = "testValue")]
    public string TestValue { get; set; }
  }
}
