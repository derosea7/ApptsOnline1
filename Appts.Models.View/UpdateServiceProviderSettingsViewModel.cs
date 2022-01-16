using System;
using Appts.Models.Api;
using System.ComponentModel.DataAnnotations;
namespace Appts.Models.View
{
  public class UpdateServiceProviderSettingsViewModel
  {
    public string DisplayName { get; set; }
    public string VanityUrl { get; set; }
    public bool RequireMyConfirmation { get; set; }
    public int SchedulingPrivacyLevel { get; set; }
    public string TimeZoneId { get; set; }
    [Phone]
    public string MobilePhone { get; set; }
    // when true, guide user through setting valid sp
    public bool IsOnboarding { get; set; }
    public string SpId { get; set; }
  }
}
