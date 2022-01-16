using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class AddLookupRequest
  {
    // aka userId
    public string PartitionId { get; set; }

    public string Key { get; set; }
    public string Value { get; set; }
  }
}
