using Newtonsoft.Json;
using System;

namespace Appts.Models.Sms
{
  public class SendSmsRequest
  {
    [JsonProperty("msg")]
    public string TextMsg { get; set; }
    [JsonProperty("phn")]
    public string ToPhoneNumber { get; set; }
    [JsonProperty("country")]
    public string CountryCode { get; set; }
    public SendSmsRequest(string toPhoneNumber, string textMsg, string countryCode = null)
    {
      TextMsg = textMsg;
      ToPhoneNumber = toPhoneNumber;
      if (countryCode == null) CountryCode = "1"; //+1 is US
    }
  }
}
