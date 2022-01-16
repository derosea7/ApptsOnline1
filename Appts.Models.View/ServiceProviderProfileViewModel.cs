using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.View
{
  public class ServiceProviderProfileViewModel
  {
    // GET requsest
    public bool FoundSp { get; set; }
    public string SpFName { get; set; }
    public string SpLName { get; set; }
    public string SpEmail { get; set; }
    public string SpDisplayName { get; set; }
    public string SpVanityUrl { get; set; }
    // postback

  }
}
