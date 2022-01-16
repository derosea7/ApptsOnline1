using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class PatchClientSettingsRequest
  {
    public ClientDoc UpdatedClient { get; set; }
  }
}
