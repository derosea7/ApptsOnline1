using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  public class AppointmentComparer : IEqualityComparer<Appointment>
  {
    public bool Equals(Appointment x, Appointment y)
    {
      if (x.StartTime == y.StartTime && x.EndTime == y.EndTime)
        return true;
      else
        return false;
    }

    public int GetHashCode(Appointment obj)
    {
      return 0;
    }
  }
}
