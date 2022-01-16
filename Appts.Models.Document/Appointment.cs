using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Appts.Models.Document
{
  /// <summary>
  /// A reserved block of time for a provider to render services to a single client.
  /// </summary>
  public class Appointment : Document
  {
    // note that userId property inherited from document here
    // refers to provider id
    // data paritoined by userId

    public Appointment()
    {
      EntityType = "Appointment";
    }

    public Appointment(DateTime start, DateTime end) : this()
    {
      StartTime = start;
      EndTime = end;
    }

    // for better testing readability
    public Appointment(string startDate, int durationMins)
    {
      DateTime start;
      bool canParse = DateTime.TryParse(startDate, out start);
      if (!canParse) throw new ArgumentException("Cannot parse startDate");

      StartTime = start;
      EndTime = start + new TimeSpan(0, durationMins, 0);
    }

    /// <summary>
    /// Client who has booked appt.
    /// </summary>
    [JsonProperty(PropertyName = "clientId")]
    public string ClientId { get; set; }

    [JsonProperty(PropertyName = "clientEmail")]
    public string ClientEmail { get; set; }

    [JsonProperty(PropertyName = "clientFName")]
    public string ClientFirstName { get; set; }

    [JsonProperty(PropertyName = "clientLName")]
    public string ClientLastName { get; set; }


    [JsonProperty(PropertyName = "clientMobile")]
    public string ClientMobile { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "clientUserRole")]
    public UserRole ClientUserRole { get; set; }

    [JsonProperty(PropertyName = "startTime")]
    public DateTime StartTime { get; set; }

    [JsonProperty(PropertyName = "endTime")]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// IANA time zone id.
    /// 
    /// All times on the appt object are converted to this tz.
    /// </summary>
    [JsonProperty(PropertyName = "timeZoneId")]
    public string TimeZoneId { get; set; }

    // not including recurring appts in first release
    //// if null, the appt does not recurr
    [JsonProperty(PropertyName = "RRULE")]
    public string RRULE { get; set; }

    // if set, this appt is a single occurence of a recurring appt,
    // with the recurrence being defined in teh appt with this id
    [JsonProperty(PropertyName = "parentId")]
    public string ParentId { get; set; }

    [JsonProperty(PropertyName = "notes")]
    public string Notes { get; set; }

    [JsonProperty(PropertyName = "reschedNotes")]
    public string RescheduleNotes { get; set; }

    [JsonProperty(PropertyName = "cancelNotes")]
    public string CancelationNotes { get; set; }

    [JsonProperty(PropertyName = "locationDetails")]
    public string LocationDetails { get; set; }

    [JsonProperty(PropertyName = "apptTypeId")]
    public string AppointmentTypeId { get; set; }

    // note: this property is the prop of a domain object,
    // whereas the other properties are more like a DTO.
    // TODO: seperate DTO and domain properties into
    // distinct objects
    public AppointmentType AppointmentType { get; set; }

    // denormalizing appointment type breif for quicker querying
    [JsonProperty(PropertyName = "apptTypeBreif")]
    public string AppointmentTypeBreif { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty(PropertyName = "status")]
    public EntityStatus Status { get; set; }

    [JsonProperty(PropertyName = "created")]
    public AuditTrail Created { get; set; }

    [JsonProperty(PropertyName = "clientTimeZone")]
    public string ClientTimeZone { get; set; }

    [JsonProperty(PropertyName = "modified")]
    public AuditTrail Modified { get; set; }

    [JsonProperty(PropertyName = "rescheduled")]
    public AuditTrail Rescheduled { get; set; }

    [JsonProperty(PropertyName = "canceled")]
    public AuditTrail Canceled { get; set; }

    [JsonProperty(PropertyName = "gcalEventId")]
    public string GcalEventId { get; set; }
    [JsonProperty(PropertyName = "spReadReciept")]
    public DateTime? SpReadReciept { get; set; }
    //todo:shorten for bytes saved on transmission
    [JsonProperty(PropertyName = "clientRead")]
    public DateTime? ClientReadReciept { get; set; }
    [JsonProperty("cliRemind")]
    public List<long> ClientApptReminders { get; set; }
    [JsonProperty("spRemind")]
    public List<long> SpApptReminders { get; set; }

    // convert all time properties of the appointment
    // to a specified time zone
    public void ConvertAllTimes(string destinationTimeZone)
    {
      StartTime = Chronotope.ConvertTimeZones(StartTime, TimeZoneId, destinationTimeZone);
      EndTime = Chronotope.ConvertTimeZones(EndTime, TimeZoneId, destinationTimeZone);
      if (Modified != null)
      {
        Modified.On = Chronotope.ConvertTimeZones((DateTime)Modified.On, TimeZoneId, destinationTimeZone);
      }
      if (Created != null)
      {
        Created.On = Chronotope.ConvertTimeZones((DateTime)Created.On, TimeZoneId, destinationTimeZone);
      }
      if (Rescheduled != null)
      {
        Rescheduled.On = Chronotope.ConvertTimeZones((DateTime)Rescheduled.On, TimeZoneId, destinationTimeZone);
      }
      if (Canceled != null)
      {
        Canceled.On = Chronotope.ConvertTimeZones((DateTime)Canceled.On, TimeZoneId, destinationTimeZone);
      }
    }

    public string FullStartDate()
    {
      return StartTime.ToString("D");
    }

    public string ReadableStartTime()
    {
      return StartTime.ToString("t");
    }

    public string ReadableEndTime()
    {
      return EndTime.ToString("t");
    }

    public string FullReadableTime()
    {
      return $"{FullStartDate()} {ReadableStartTime()} - {ReadableEndTime()}";
    }

    public override string ToString()
    {
      string summary = $"{StartTime.ToShortDateString()} | {StartTime.ToString("t")} to {EndTime.ToString("t")} | {AppointmentTypeBreif}";
      return summary;
    }
  }
}
