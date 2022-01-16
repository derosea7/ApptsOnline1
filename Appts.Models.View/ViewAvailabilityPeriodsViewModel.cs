using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.View
{
  public class ViewAvailabilityPeriodsViewModel
  {
    public List<AvailabilityPeriod> Periods { get; set; }

    public string PeriodsJson { get; set; }
    public string PeriodDatesJson { get; set; }
  }
}
