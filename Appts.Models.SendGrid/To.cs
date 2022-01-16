using Newtonsoft.Json;
using System;

namespace Appts.Models.SendGrid
{
  public class To
  {
    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    public To(string email, string name)
    {
      Email = email;
      Name = name;
    }
  }
}
