using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class NonceHelper
  {
    public int GetRandom32BitSignedInt()
    {
      Random rand = new Random();
      return rand.Next();
    }
  }
}
