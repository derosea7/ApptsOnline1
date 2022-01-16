using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  /// <summary>
  /// Contains settings for appointments.
  /// </summary>
  public class AppointmentType : Document
  {
    public AppointmentType()
    {
      this.EntityType = "ApptType";
    }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    /// <summary>
    /// When true, record will be made available to public.
    /// </summary>
    [JsonProperty(PropertyName = "isActive")]
    public bool IsActive { get; set; }
    /// <summary>
    /// True when an appointment type should be considered deleted by a user.
    /// Must soft delete at this point because of dependencies on appt type.
    /// </summary>
    [JsonProperty(PropertyName = "deleted")]
    public bool Deleted { get; set; }

    [JsonProperty(PropertyName = "duration")]
    public TimeSpan Duration { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    //public bool Public { get; set;} // display on main booking page

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    // will be filled in for
    // we spec location,
    // customer calls us
    // web conf
    [JsonProperty(PropertyName = "locationDetails")]
    public string LocationDetails { get; set; }

    // not including recur in 1st release
    //// recur
    //[JsonConverter(typeof(StringEnumConverter))]
    //[JsonProperty(PropertyName = "recurRequirement")]
    //public RecurrenceRequirement RecurrenceRequirement { get; set; }

    // minimum time required
    [JsonProperty(PropertyName = "cancelationNotice")]
    public TimeSpan CancelationNotice { get; set; }

    // minimum time required
    [JsonProperty(PropertyName = "rescheduleNotice")]
    public TimeSpan RescheduleNotice { get; set; }

    // minimum time required
    [JsonProperty(PropertyName = "minimumNotice")]
    public TimeSpan MinimumNotice { get; set; }

    // minimum time required
    [JsonProperty(PropertyName = "maximumNotice")]
    public TimeSpan MaximumNotice { get; set; }

    // prep time
    [JsonProperty(PropertyName = "bufferBefore")]
    public TimeSpan BufferBefore { get; set; }

    // time allowed after appt before next
    [JsonProperty(PropertyName = "bufferAfter")]
    public TimeSpan BufferAfter { get; set; }

    /// <summary>
    /// Summary of appointment type.
    /// </summary>
    /// <returns>Human readable summary of appointment type.</returns>
    public string ToFormattedDurationString()
    {
      string displayText = "";
      if (Duration.Hours > 0)
      {
        displayText += $" {Duration:%h} hr{(Duration.TotalMinutes >= 120 ? "s" : "")}";
      }
      displayText += $" {Duration:%m} mins";

      return displayText;
    }

    /// <summary>
    /// Summary of appointment type.
    /// </summary>
    /// <returns>Human readable summary of appointment type.</returns>
    public override string ToString()
    {
      string displayText = $"{Name} |";
      if (Duration.Hours > 0)
      {
        displayText += $" {Duration:%h} hr{(Duration.TotalMinutes >= 120 ? "s" : "")}";
      }
      displayText += $" {Duration:%m} mins";

      return displayText;
    }
  }
}
