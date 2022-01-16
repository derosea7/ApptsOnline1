using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class UpdateTimeZoneRequest
  {
    public string TimeZoneId { get; set; }

    public string ServiceProviderId { get; set; }
    public string ClientUserId { get; set; }
  }
}
