using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class ContiguousAvailabilityBlock
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ContiguousAvailabilityBlock(DateTime start, DateTime end)
    {
      StartDate = start;
      EndDate = end;
    }
    public override string ToString()
    {
      return ($"{StartDate.ToString()} to {EndDate.ToString()}");
    }
  }
}
