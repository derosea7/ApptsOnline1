using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class AddConnectedCalendarResponse
  {
    public bool SavedCalendarSuccessfully { get; set; }
    public bool CachedTokenSuccessfully { get; set; }
  }
}
