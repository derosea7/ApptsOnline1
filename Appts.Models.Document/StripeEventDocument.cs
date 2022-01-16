using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Document
{
  public class StripeEventDocument : Document
  {
    public string EventType { get; set; }
    public StripeEventDocument(string eventType)
    {
      EntityType = "StripeEvent";
      EventType = eventType;
    }
  }
}