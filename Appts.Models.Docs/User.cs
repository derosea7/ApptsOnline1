using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Docs
{
  public class User : Doc
  {
    public User()
    {
      EType = "User";
    }
    //[JsonProperty(PropertyName = "objid")]
    //public string ObjectId { get; set; }
    //[JsonProperty(PropertyName = "nameid")]
    //public string NameId { get; set; }

      // from b2c ad, guid
    [JsonProperty(PropertyName = "uid")]
    public string Uid { get; set; }

    [JsonProperty(PropertyName = "email")]
    public string Email { get; set; }
    [JsonProperty(PropertyName = "fname")]
    public string FirstName { get; set; }
    [JsonProperty(PropertyName = "lname")]
    public string LastName { get; set; }
  }
}
