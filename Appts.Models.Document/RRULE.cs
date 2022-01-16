using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Appts.Models.Document
{
  /**
   * https://tools.ietf.org/html/rfc5545#section-3.8.5
   * 
   * Examples
   *   - FREQ=MONTHLY;INTERVAL=3;BYMONTHDAY=20;COUNT=5
   *     occurs on the 20th of the month, every 3 months,
   *     for 5 instances.
   *     
   * FREQ:
   *   - DAILY, WEEKLY, MONTHLY, YEARLY
   * INTERVAL:
   *   - specifies the interval between 2 occurences
   *   - if interval left out of RRULE string, then default value is 1
   *   
   * COUNT: number of times the event recurs
   * UNTIL: The end date for the event.
   * 
   * COUNT and UNTIL are mutually exclusive; one or the other.
   * 
   * https://www.rahulsingla.com/blog/2010/12/parsing-ical-rrule/
   */
  public class RRULE
  {
    public RRULE_FREQ Frequency { get; set; }
    public int Interval { get; set; }
    public List<RRULE_DAY> ByDay { get; set; }

    // MONTHLY Properties
    public byte EveryNthDayOfMonth { get; set; }
    public RRULE_DAY Monthly_ByDay_Day { get; set; }
    public List<byte> ByMonthDay { get; set; }

    public int Count { get; set; }
    public DateTime Until { get; set; }

    public RRULE(string rule)
    {
      ByMonthDay = new List<byte>();
      ParseRule(rule);
    }

    public void ParseRule(string rule)
    {
      var rules = rule.Split(';');
      string key, value;
      string[] kvRule;
      foreach (string r in rules)
      {
        kvRule = r.Split('=');
        key = kvRule[0];
        value = kvRule[1];

        SetRule(key, value);
      }

      // BYDAY was not included, therefor add all days
      if (Frequency == RRULE_FREQ.WEEKLY && ByDay == null)
      {
        ByDay = GetAllDays();
      }

      // FREQ=DAILY without INTERVAL set; interval should be 1 for everyday
      if (Interval == 0)
      {
        Interval = 1;
      }

      // second pass over set field values to ensure
      // nothing set invalid?
      if (Frequency == RRULE_FREQ.MONTHLY)
      {
        if (ByMonthDay.Count > 0 && Monthly_ByDay_Day != RRULE_DAY.Undefined)
          throw new ArgumentException("Both BYDAY and BYMONTH day cannot be present");
      }
    }

    private List<RRULE_DAY> GetAllDays()
    {
      return new List<RRULE_DAY>()
        {
          RRULE_DAY.MO,
          RRULE_DAY.TU,
          RRULE_DAY.WE,
          RRULE_DAY.TH,
          RRULE_DAY.FR,
          RRULE_DAY.SA,
          RRULE_DAY.SU
        };
    }

    private void SetRule(string key, string value)
    {
      switch (key)
      {
        case "FREQ":
          SetFrequencyEnum(value);
          break;

        case "INTERVAL":
          Interval = System.Convert.ToInt32(value);
          break;

        case "COUNT":
          Count = System.Convert.ToInt32(value);
          break;

        case "BYDAY":
          SetByDay(value);
          break;

        case "BYMONTHDAY":
          string[] monthDayStrings = value.Split(',');
          ByMonthDay = new List<byte>();
          foreach (string monthDay in monthDayStrings)
          {
            ByMonthDay.Add(System.Convert.ToByte(monthDay));
          }
          //ByMonthDay = System.Convert.ToByte(value);
          break;

        case "UNTIL":
          DateTime until;
          if (DateTime.TryParseExact(
            value, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out until))
            Until = until;
          else
            throw new ArgumentException("UNTIL cannot be parsed");
          break;

        default:
          break;
      }


    }

    private void SetByDay(string value)
    {
      switch (Frequency)
      {
        case RRULE_FREQ.WEEKLY:
          string[] dayStrings = value.Split(',');
          ByDay = new List<RRULE_DAY>();
          foreach (string day in dayStrings)
          {
            ByDay.Add(GetDayEnum(day));
          }
          break;

        case RRULE_FREQ.MONTHLY:
          // example: FREQ=MONTHLY;BYDAY=3MO
          // every 3rd monday of the month, int can be between 1 and 5
          EveryNthDayOfMonth = Convert.ToByte(value.Substring(0, 1));
          Monthly_ByDay_Day = GetDayEnum(value.Substring(1, 2));

          break;

          // not handling for now
        case RRULE_FREQ.YEARLY:
          break;
        default:
          break;
      }
    }

    private RRULE_DAY GetDayEnum(string day)
    {
      switch (day)
      {
        case "MO":
          return RRULE_DAY.MO;

        case "TU":
          return RRULE_DAY.TU;

        case "WE":
          return RRULE_DAY.WE;

        case "TH":
          return RRULE_DAY.TH;

        case "FR":
          return RRULE_DAY.FR;

        case "SA":
          return RRULE_DAY.SA;

        case "SU":
          return RRULE_DAY.SU;

        default:
          throw new ArgumentException("BYDAY day value is invalid");
      }
    }

    private void SetFrequencyEnum(string frequency)
    {
      switch (frequency)
      {
        case "DAILY":
          Frequency = RRULE_FREQ.DAILY;
          break;

        case "WEEKLY":
          Frequency = RRULE_FREQ.WEEKLY;
          break;

        case "MONTHLY":
          Frequency = RRULE_FREQ.MONTHLY;
          break;

        case "YEARLY":
          Frequency = RRULE_FREQ.YEARLY;
          break;

        default:
          throw new ArgumentException("The FREQ value is not valid");
      }
    }
  }
}
