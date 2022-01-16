using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  /// <summary>
  /// Returns true if 2 availability blocks start and end at the same time.
  /// </summary>
  public class AvailabilityBlockComparer : IEqualityComparer<AvailabilityBlock>
  {
    // returns true if timespans in the availability blocks represent that same time
    public bool Equals(AvailabilityBlock x, AvailabilityBlock y)
    {
      if (x.StartTime == y.StartTime && x.EndTime == y.EndTime)
        return true;
      else
        return false;
    }

    public int GetHashCode(AvailabilityBlock obj)
    {
      return 0;
    }
  }
}
