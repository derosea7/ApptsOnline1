using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  public class ByteListComparer : IEqualityComparer<byte>
  {
    public bool Equals(byte x, byte y)
    {
      if (x == y) return true;
      else return false;
    }

    public int GetHashCode(byte obj)
    {
      return 0;
    }
  }
}
