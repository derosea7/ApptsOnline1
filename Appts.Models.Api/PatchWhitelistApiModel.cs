using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Api
{
  public class PatchWhitelistApiModel
  {
    public string ServiceProviderId { get; set; }
    public List<string> WhitelistEntries { get; set; }
  }
}
