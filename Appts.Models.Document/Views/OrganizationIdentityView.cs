using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document.Views
{
  public class OrganizationIdentityView
  {
    [JsonProperty(PropertyName = "orgId")]
    public string OrganizationId { get; set; }

    [JsonProperty(PropertyName = "roles")]
    public List<string> Roles { get; set; }
  }
}
