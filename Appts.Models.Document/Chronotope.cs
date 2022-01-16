using Newtonsoft.Json;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Domain
{
  /// <summary>
  /// Time and space; a date, time and time zone.
  /// 
  /// https://stackoverflow.com/questions/39475617/convert-system-datetime-to-nodatime-zoneddatetime
  /// </summary>
  public class Chronotope
  {
    [JsonProperty(PropertyName = "dateTime")]
    public DateTime DateTime { get; set; }
    /// <summary>
    /// IANA timezone id.
    /// </summary>

    [JsonProperty(PropertyName = "ianaTimeZone")]
    public string IanaTimeZone { get; set; }

    /// <summary>
    /// Generated server-side; meant for check and balance
    /// on time and place recieved from front end.
    /// </summary>
    [JsonProperty(PropertyName = "systemUtcDateTime")]
    public DateTime SystemUtcDateTime { get; set; }

    /// <summary>
    /// User at the time and place.
    /// </summary>
    [JsonProperty(PropertyName = "userId")]
    public string UserId { get; set; }
    /// <summary>
    /// Create now in a specified time zone.
    /// </summary>
    /// <param name="timeZone">Time zone to get now in</param>
    /// <returns>Now as DateTime in a specified time zone.</returns>
    public static DateTime CreateZonedTime(string timeZone)
    {
      //var localTime = new LocalDateTime();
      var targetTzProvider = DateTimeZoneProviders.Tzdb[timeZone];

      IClock clock = SystemClock.Instance;
      Instant now = clock.GetCurrentInstant();

      ZonedDateTime zonedNow = now.InZone(targetTzProvider);

      return zonedNow.ToDateTimeUnspecified();
    }

    public static DateTime ConvertTimeZones(DateTime sourceTime,
      string sourceTimeZone, string destinationTimeZone)
    {
      var localSourceTime = LocalDateTime.FromDateTime(sourceTime);
      var sourceTzProvider = DateTimeZoneProviders.Tzdb[sourceTimeZone];
      var sourceZonedTime = sourceTzProvider.AtStrictly(localSourceTime);

      // now convert
      var destinationTzProvider = DateTimeZoneProviders.Tzdb[destinationTimeZone];
      var destinationZonedTime = sourceZonedTime.WithZone(destinationTzProvider);

      return destinationZonedTime.ToDateTimeUnspecified();
    }

    public static TimeSpan DifferenceBetweenTimeZones(DateTime sourceTime,
      string sourceTimeZone, string destinationTimeZone)
    {
      DateTime convertedTime = ConvertTimeZones(sourceTime, sourceTimeZone, destinationTimeZone);

      // returns magnitude equivalent to UTC offsets (i.e. EST -5, MST - 7, EST < MST)
      TimeSpan span = convertedTime.Subtract(sourceTime);

      return span;
    }

    //public static TimeSpan GetDifferenceBetweenTimeZones()
  }
}
