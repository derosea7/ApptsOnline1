using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  public class ByDayComparer : IEqualityComparer<RRULE_DAY>
  {
    public bool Equals(RRULE_DAY x, RRULE_DAY y)
    {
      if (x == y)
        return true;
      else
        return false;
    }

    public int GetHashCode(RRULE_DAY obj)
    {
      return 0;
    }
  }
}
