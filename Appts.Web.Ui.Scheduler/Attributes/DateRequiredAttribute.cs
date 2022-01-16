using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Attributes
{
  public class DateRequiredAttribute : RequiredAttribute
  {
    public DateRequiredAttribute()
    {
      ErrorMessage = "Must select a date";
    }
  }
}
