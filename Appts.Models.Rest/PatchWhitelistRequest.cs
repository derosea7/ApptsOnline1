using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Api;

namespace Appts.Models.Rest
{
  public class PatchWhitelistRequest
  {
    public string ServiceProviderId { get; set; }
    public List<string> WhitelistEntries { get; set; }
    //public PatchWhitelistApiModel WhitelistPatch { get; set; }
  }
}
