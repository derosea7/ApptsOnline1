using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  // TODO: find home for this extension
  public static class DateTimeExtensions
  {
    ///<summary>Gets the first week day following a date.</summary>
    ///<param name="date">The date.</param>
    ///<param name="dayOfWeek">The day of week to return.</param>
    ///<returns>The first dayOfWeek day following date, or date if it is on dayOfWeek.</returns>
    public static DateTime Next(this DateTime date, DayOfWeek dayOfWeek)
    {
      return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
    }
  }



  public class RecurringAppointment
  {
    /**
     How do i get the nth weekday of the month?
For ex.: 
2nd Monday of "July 2010" = 07/12/2010.
     */
    //(I need to subtract one because date.Next(dayOfWeek) is already the first occurrence of that day)
    public static DateTime GetNthWeekofMonth(DateTime date, int nthWeek, DayOfWeek dayOfWeek)
    {
      return date.Next(dayOfWeek).AddDays((nthWeek - 1) * 7);
    }

    // start must be the first occurrence of the rrule
    public static List<Appointment> GetAppointments(Appointment originalAppt)
    {
      var appts = new List<Appointment>();
      Appointment appt;
      RRULE rule = new RRULE(originalAppt.RRULE);


      switch (rule.Frequency)
      {
        case RRULE_FREQ.Undefined:
          break;
        case RRULE_FREQ.DAILY:
          HandleDaily(originalAppt, rule, appts);
          break;

        case RRULE_FREQ.WEEKLY:
          HandleWeekly(originalAppt, rule, appts);
          break;

        case RRULE_FREQ.MONTHLY:
          HandleMonthly(originalAppt, rule, appts);
          break;

        case RRULE_FREQ.YEARLY:
          break;
        default:
          break;
      }

      return appts;
    }

    #region "DAILY"

    private static void HandleDaily(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      if (rule.Count != 0)
      {
        HandleDailyWithCount(originalAppt, rule, appts);
      }
      else
      {
        HandleDailyWithUntil(originalAppt, rule, appts);
      }
    }

    private static void HandleDailyWithCount(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {

    }

    private static void HandleDailyWithUntil(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      // --------------------------------------------------------------
      //string rule = "FREQ=DAILY;INTERVAL=2;UNTIL=20191105T000000";

      Appointment appt;
      DateTime lastStart = originalAppt.StartTime.AddDays(rule.Interval);
      DateTime occurrence = originalAppt.StartTime;
      while (occurrence <= rule.Until)
      {
        appt = new Appointment()
        {
          Id = Guid.NewGuid().ToString(),
          ParentId = originalAppt.Id,
          StartTime = occurrence.Date + originalAppt.StartTime.TimeOfDay,
          EndTime = occurrence.Date + originalAppt.EndTime.TimeOfDay,
          AppointmentTypeId = originalAppt.AppointmentTypeId
        };
        appts.Add(appt);

        occurrence = occurrence.AddDays(rule.Interval);
      }
    }

    #endregion

    #region "WEEKLY"

    private static void HandleWeekly(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      if (rule.Count != 0)
      {
        HandleWeeklyByDayWithCount(originalAppt, rule, appts);
      }
      else
      {
        HandleWeeklyByDayWithUntil(originalAppt, rule, appts);
      }
    }

    private static DateTime GetBeginningOfWeek(DateTime dateInWeek)
    {
      int dayOfWeek = (int)dateInWeek.DayOfWeek;
      DateTime startOfWeek = dateInWeek.AddDays(-(int)dateInWeek.DayOfWeek);

      return startOfWeek;
    }

    private static void HandleWeeklyByDayWithUntil(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      appts.Clear();
      Appointment appt;
      List<DayOfWeek> byDay = new List<DayOfWeek>();
      DateTime occurrence = GetBeginningOfWeek(originalAppt.StartTime);

      foreach (RRULE_DAY day in rule.ByDay)
      {
        byDay.Add(GetDowFromRRULE_Day(day));
      }
      int offsetFromSunday;
      while (occurrence <= rule.Until)
      {
        foreach (DayOfWeek day in byDay)
        {
          offsetFromSunday = (int)day;
          DateTime concreteDow = occurrence.AddDays(offsetFromSunday);

          if (concreteDow > rule.Until) break;

          appt = new Appointment()
          {
            Id = Guid.NewGuid().ToString(),
            ParentId = originalAppt.Id,
            StartTime = concreteDow.Date + originalAppt.StartTime.TimeOfDay,
            EndTime = concreteDow.Date + originalAppt.EndTime.TimeOfDay,
            AppointmentTypeId = originalAppt.AppointmentTypeId
          };
          appts.Add(appt);
        }

        occurrence = occurrence.AddDays(7 * rule.Interval);
      }
    }

    private static void HandleWeeklyByDayWithCount(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      Appointment appt;
      List<DayOfWeek> byDay = new List<DayOfWeek>();
      DateTime lastWeekStart = GetBeginningOfWeek(originalAppt.StartTime);

      // convert BYDAY=MO,WE; to DateTime.DayOfWeek
      foreach (RRULE_DAY day in rule.ByDay)
      {
        byDay.Add(GetDowFromRRULE_Day(day));
      }

      int offsetFromSunday;
      for (int i = 1; i < rule.Count + 1; i++)
      {
        foreach (DayOfWeek day in byDay)
        {
          offsetFromSunday = (int)day;
          DateTime concreteDow = lastWeekStart.AddDays(offsetFromSunday);

          appt = new Appointment()
          {
            Id = Guid.NewGuid().ToString(),
            ParentId = originalAppt.Id,
            StartTime = concreteDow.Date + originalAppt.StartTime.TimeOfDay,
            EndTime = concreteDow.Date + originalAppt.EndTime.TimeOfDay,
            AppointmentTypeId = originalAppt.AppointmentTypeId
          };
          appts.Add(appt);
        }

        lastWeekStart = lastWeekStart.AddDays(7 * rule.Interval);
      }
    }

    #endregion

    #region "MONTHLY"

    private static void HandleMonthly(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      if (rule.ByMonthDay.Count > 0)
      {
        HandleByMonthDay(originalAppt, rule, appts);
      }
      else if (rule.Monthly_ByDay_Day != RRULE_DAY.Undefined)
      {
        HandleByDay(originalAppt, rule, appts);
      }
    }

    private static void HandleByDay(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      if (rule.Count != 0)
      {
        HandleByDayWithCount(originalAppt, rule, appts);
      }
      else
      {
        HandleByDayWithUntil(originalAppt, rule, appts);
      }
    }

    private static void HandleByMonthDay(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      if (rule.Count != 0)
      {
        HandleByMonthDayWithCount(originalAppt, rule, appts);
      }
      else
      {
        HandleByMonthDayWithUntil(originalAppt, rule, appts);
      }
    }

    private static DayOfWeek GetDowFromRRULE_Day(RRULE_DAY day)
    {
      switch (day)
      {
        case RRULE_DAY.MO:
          return DayOfWeek.Monday;

        case RRULE_DAY.TU:
          return DayOfWeek.Tuesday;

        case RRULE_DAY.WE:
          return DayOfWeek.Wednesday;

        case RRULE_DAY.TH:
          return DayOfWeek.Thursday;

        case RRULE_DAY.FR:
          return DayOfWeek.Friday;

        case RRULE_DAY.SA:
          return DayOfWeek.Saturday;

        case RRULE_DAY.SU:
          return DayOfWeek.Sunday;

        default:
          throw new ArgumentException("Invalid RRULE_DAY");
      }
    }

    private static void HandleByDayWithCount(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      Appointment appt;
      DayOfWeek dow = GetDowFromRRULE_Day(rule.Monthly_ByDay_Day);
      DateTime occurrence;
      DateTime day1OfOccurrence;
      DateTime nthDayOfMonth;

      for (int i = 0; i < rule.Count; i++)
      {
        // increment to get next instance
        occurrence = originalAppt.StartTime.AddMonths(rule.Interval * i);
        day1OfOccurrence = new DateTime(occurrence.Year, occurrence.Month, 1);
        nthDayOfMonth = GetNthWeekofMonth(day1OfOccurrence, (int)rule.EveryNthDayOfMonth, dow);

        appt = new Appointment()
        {
          Id = Guid.NewGuid().ToString(),
          ParentId = originalAppt.Id,
          StartTime = nthDayOfMonth.Date + originalAppt.StartTime.TimeOfDay,
          EndTime = nthDayOfMonth.Date + originalAppt.EndTime.TimeOfDay,
          AppointmentTypeId = originalAppt.AppointmentTypeId
        };
        appts.Add(appt);
      }
    }

    private static void HandleByDayWithUntil(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      Appointment appt;
      DayOfWeek dow = GetDowFromRRULE_Day(rule.Monthly_ByDay_Day);
      DateTime nextStart = originalAppt.StartTime;
      DateTime day1OfNextStart = new DateTime(nextStart.Year, nextStart.Month, 1);
      DateTime nextNthDayOfMonth = GetNthWeekofMonth(
        day1OfNextStart, (int)rule.EveryNthDayOfMonth, dow);

      while (nextNthDayOfMonth <= rule.Until)
      {
        appt = new Appointment()
        {
          Id = Guid.NewGuid().ToString(),
          ParentId = originalAppt.Id,
          StartTime = nextNthDayOfMonth.Date + originalAppt.StartTime.TimeOfDay,
          EndTime = nextNthDayOfMonth.Date + originalAppt.EndTime.TimeOfDay,
          AppointmentTypeId = originalAppt.AppointmentTypeId
        };
        appts.Add(appt);

        // increment to get next instance
        nextStart = nextNthDayOfMonth.AddMonths(rule.Interval);
        day1OfNextStart = new DateTime(nextStart.Year, nextStart.Month, 1);
        nextNthDayOfMonth = GetNthWeekofMonth(
          day1OfNextStart, (int)rule.EveryNthDayOfMonth, dow);
      }
    }



    private static void HandleByMonthDayWithUntil(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      appts.Clear();
      Appointment appt;
      DateTime lastStart = originalAppt.StartTime;
      DateTime nextNthDay;

      while (lastStart <= rule.Until)
      {
        foreach (byte monthDay in rule.ByMonthDay)
        {
          nextNthDay = new DateTime(lastStart.Year, lastStart.Month, monthDay);
          appt = new Appointment()
          {
            Id = Guid.NewGuid().ToString(),
            ParentId = originalAppt.Id,
            StartTime = nextNthDay.Date + originalAppt.StartTime.TimeOfDay,
            EndTime = nextNthDay.Date + originalAppt.EndTime.TimeOfDay,
            AppointmentTypeId = originalAppt.AppointmentTypeId
          };
          appts.Add(appt);

        }
        lastStart = lastStart.AddMonths(rule.Interval);
      }
    }

    private static void HandleByMonthDayWithCount(
      Appointment originalAppt, RRULE rule, List<Appointment> appts)
    {
      appts.Clear();
      DateTime nextNthDay;
      DateTime nextInterval = originalAppt.StartTime;
      Appointment appt;

      // i being 0 first the first iteration means interval * 0 = 0, 
      // and so no months are added, and so the first appt is the
      // original appt
      for (int i = 0; i < rule.Count; i++)
      {
        foreach (byte monthDay in rule.ByMonthDay)
        {
          nextInterval = originalAppt.StartTime.AddMonths(rule.Interval * i);
          nextNthDay = new DateTime(nextInterval.Year, nextInterval.Month, monthDay);
          appt = new Appointment()
          {
            Id = Guid.NewGuid().ToString(),
            ParentId = originalAppt.Id,
            StartTime = nextNthDay.Date + originalAppt.StartTime.TimeOfDay,
            EndTime = nextNthDay.Date + originalAppt.EndTime.TimeOfDay,
            AppointmentTypeId = originalAppt.AppointmentTypeId
          };
          appts.Add(appt);
        }
      }
    }

    #endregion

  }
}
