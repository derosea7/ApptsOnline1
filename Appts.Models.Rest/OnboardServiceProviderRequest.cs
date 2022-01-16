using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class OnboardServiceProviderRequest
  {
    public string ServiceProviderId { get; set; }
    public string DisplayName { get; set; }
    public string VanityUrl { get; set; }
    public bool RequireMyConfirmation { get; set; }
    public int SchedulingPrivacyLevel { get; set; }
    public string TimeZoneId { get; set; }

    public string Email { get; set; }
  }
}
