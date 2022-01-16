using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class DeleteAvailabilityPeriodRequest
  {
    public string PeriodId { get; set; }
    public string UserId { get; set; }
  }
}
