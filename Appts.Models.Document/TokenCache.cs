using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class TokenCache : Document
  {
    public TokenCache()
    {
      EntityType = "TokenCache";
    }

    [JsonProperty(PropertyName = "accessToken")]
    public string AccessToken { get; set; }

    [JsonProperty(PropertyName = "refreshToken")]
    public string RefreshToken { get; set; }

    [JsonProperty(PropertyName = "expiresInSeconds")]
    public long ExpiresInSeconds { get; set; }

    [JsonProperty(PropertyName = "scopes")]
    public string[] Scopes { get; set; }
  }
}
