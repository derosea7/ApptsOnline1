using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public enum RRULE_FREQ
  {
    Undefined,
    DAILY,
    WEEKLY,
    MONTHLY,
    YEARLY
  }
  public enum RRULE_FREQ_MONTHLY
  {
    Undefined,
    BYMONTHDAY,
    BYDAY
  }
}
