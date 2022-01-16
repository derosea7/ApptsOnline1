using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class ClientDoc : Document
  {
    public ClientDoc()
    {
      EntityType = "Client";
    }
    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName { get; set; }
    [JsonProperty(PropertyName = "timeZoneId")]
    public string TimeZoneId { get; set; }

    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }

    [JsonProperty(PropertyName = "fname")]
    public string FirstName { get; set; }

    [JsonProperty(PropertyName = "lname")]
    public string LastName { get; set; }

    [JsonProperty(PropertyName = "mobilePhone")]
    public string MobilePhone { get; set; }

    [JsonProperty(PropertyName = "address")]
    public string Address { get; set; }
    [JsonProperty(PropertyName = "address2")]
    public string Address2 { get; set; }
    [JsonProperty(PropertyName = "city")]
    public string City { get; set; }
    [JsonProperty(PropertyName = "stateCode")]
    public string StateCode { get; set; }
    [JsonProperty(PropertyName = "zipCode")]
    public string ZipCode { get; set; }
  }
}
