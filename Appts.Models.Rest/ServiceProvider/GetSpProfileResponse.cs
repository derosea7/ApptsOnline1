using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest.ServiceProvider
{
  public class GetSpProfileResponse
  {
    public ServiceProviderDocument Sp { get; set; }
    public List<AvailabilityPeriod> AvailabilityPeriods { get; set; }
  }
}
