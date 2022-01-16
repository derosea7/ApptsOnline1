using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Api
{
  public class AppointmentTypeForSelectionApiModel
  {
    /*
         c.name
    , c.description
    , c.duration
    , c.location
    , c.id
 */

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "duration")]
    public TimeSpan Duration { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    //public bool Public { get; set;} // display on main booking page

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    /// <summary>
    /// Unique identifier for the record. Random guid for now.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "breif")]
    public string AppointmentTypeBreif { get; set; }
  }
}
