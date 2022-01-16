using Appts.Models.Api;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class GetPeriodDatesResponse
  {
    public List<PeriodDatesApiModel> PeriodDates { get; set; }
  }
}
