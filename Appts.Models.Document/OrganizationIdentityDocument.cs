using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Appts.Models.Document
{
  public class OrganizationIdentityDocument : Document
  {
    public OrganizationIdentityDocument()
    {
      EntityType = "OrgId";
    }

    [JsonProperty(PropertyName = "orgId")]
    public string OrganizationId { get; set; }

    [JsonProperty(PropertyName = "roles")]
    public List<string> Roles { get; set; }
  }
}
