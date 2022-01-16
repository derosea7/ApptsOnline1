using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Appts.Models.Domain;
using Appts.Extensions;

namespace Appts.Models.Document
{
  public class ServiceProviderDocument : Document
  {
    public ServiceProviderDocument()
    {
      EntityType = "ServiceProvider";
    }
    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName { get; set; }

    [JsonProperty(PropertyName = "vanityUrl")]
    public string VanityUrl { get; set; }

    [JsonProperty(PropertyName = "requireMyConfirmation")]
    public bool RequireMyConfirmation { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "schedulingPrivacyLevel")]
    public SchedulingPrivacyLevel SchedulingPrivacyLevel { get; set; }

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

    public void CleanVanityUrl()
    {
      if (string.IsNullOrEmpty(VanityUrl))
        return;

      var goodChars = new List<char>();
      for (int i = 0; i < VanityUrl.Length; i++)
      {
        if ((char.IsLetter(VanityUrl[i])) || ((char.IsNumber(VanityUrl[i]))))
          goodChars.Add(VanityUrl[i]);
      }

      VanityUrl = new string(goodChars.ToArray());
    }

    public bool IsVanityUrlValid()
    {
      return VanityUrl.IsAlphaNum();
    }
  }
}
