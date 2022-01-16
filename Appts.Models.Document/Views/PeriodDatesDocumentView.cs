using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document.Views
{
  public class PeriodDatesDocumentView
  {
    [JsonProperty(PropertyName = "startDate")]
    public DateTime StartDate { get; set; }

    [JsonProperty(PropertyName = "endDate")]
    public DateTime EndDate { get; set; }
  }
}
