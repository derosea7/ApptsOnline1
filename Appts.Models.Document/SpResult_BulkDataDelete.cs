using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class SpResult_BulkDataDelete : Document
  {
    [JsonProperty("deleted")]
    public int Deleted { get; set; }
    [JsonProperty("continuation")]
    public bool Continuation { get; set; }
  }
}
