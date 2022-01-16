using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid
{
  public class SendMailRequest
  {
    [JsonProperty(PropertyName = "personalizations")]
    public List<Personalizations> Personalizations { get; set; }
    [JsonProperty(PropertyName = "from")]
    public To From { get; set; }
    [JsonProperty(PropertyName = "reply_to")]
    public To ReplyTo { get; set; }
    [JsonProperty(PropertyName = "template_id")]
    public string TemplateId { get; set; }
  }
}
