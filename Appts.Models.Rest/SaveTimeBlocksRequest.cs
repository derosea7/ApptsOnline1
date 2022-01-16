using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class SaveTimeBlocksRequest
  {
    public List<AvailabilityBlock> Blocks { get; set; }
  }
}
