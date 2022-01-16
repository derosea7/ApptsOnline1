using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  /**
   * 
   * https://tools.ietf.org/html/rfc5545#section-3.8.5
   */
  public class RRULEShould
  {
    [Fact]
    public void ParseRule_Simple0()
    {
      string rule = "FREQ=YEARLY;INTERVAL=2;COUNT=3";

      var sut = new RRULE(rule);


      Assert.Equal(RRULE_FREQ.YEARLY, sut.Frequency);
      Assert.Equal(2, sut.Interval);
      Assert.Equal(3, sut.Count);
    }

    [Fact]
    public void Compare2ByDayLists()
    {
      var comparer = new ByDayComparer();
      var moWe = new List<RRULE_DAY>()
      {
        RRULE_DAY.MO,
        RRULE_DAY.WE
      };

      var moWeFr = new List<RRULE_DAY>()
      {
        RRULE_DAY.MO,
        RRULE_DAY.WE,
        //RRULE_DAY.FR
      };

      Assert.Equal<RRULE_DAY>(moWe, moWeFr, comparer);
    }

    #region "DAILY"

    /// <summary>
    /// If FREQ=DAILY and INTERVAL is not set, then assume 
    /// every day, i.e. interval = 1.
    /// </summary>
    [Fact]
    public void ParseRule_Daily_NoInterval()
    {
      string rule = "FREQ=DAILY;COUNT=3";
      var sut = new RRULE(rule);

      Assert.Equal(1, sut.Interval);
    }

    #endregion

    #region "WEEKLY"

    // when BYDAY property is absent on WEEKLY, interpret
    // it to mean all days of that week
    // if byday is specified, then only include those days.
    [Fact]
    public void ParseRule_Weekly_MoAndWe()
    {
      var comparer = new ByDayComparer();
      string rule = "FREQ=WEEKLY;INTERVAL=2;BYDAY=MO,WE;COUNT=3";

      var sut = new RRULE(rule);

      var moWe = new List<RRULE_DAY>()
      {
        RRULE_DAY.MO,
        RRULE_DAY.WE
      };

      Assert.Equal<RRULE_DAY>(moWe, sut.ByDay, comparer);
    }

    // when BYDAY property is absent on WEEKLY, interpret
    // it to mean all days of that week
    [Fact]
    public void ParseRule_Weekly_AllWeekDays()
    {
      var comparer = new ByDayComparer();
      string rule = "FREQ=WEEKLY;INTERVAL=2;COUNT=3";

      var sut = new RRULE(rule);

      var moWe = new List<RRULE_DAY>()
      {
        RRULE_DAY.MO,
        RRULE_DAY.TU,
        RRULE_DAY.WE,
        RRULE_DAY.TH,
        RRULE_DAY.FR,
        RRULE_DAY.SA,
        RRULE_DAY.SU
      };

      Assert.Equal<RRULE_DAY>(moWe, sut.ByDay, comparer);
    }

    #endregion

    #region "MONTHLY"

    [Fact]
    public void ParseRule_Monthly_Every3rdMonday()
    {
      string rule = "FREQ=MONTHLY;INTERVAL=3;BYDAY=3MO;COUNT=5";

      var sut = new RRULE(rule);

      Assert.Equal(3, sut.EveryNthDayOfMonth);
      Assert.Equal(RRULE_DAY.MO, sut.Monthly_ByDay_Day);
    }

    [Fact]
    public void ParseRule_Monthly_20thOfMonth()
    {
      var comparer = new ByteListComparer();
      string rule = "FREQ=MONTHLY;INTERVAL=3;BYMONTHDAY=20;COUNT=5";

      var sut = new RRULE(rule);

      var expected = new List<byte>()
      {
        20
      };

      Assert.Equal<byte>(expected, sut.ByMonthDay, comparer);
    }

    [Fact]
    public void ParseRule_Monthly_20thAnd22ndOfMonth()
    {
      var comparer = new ByteListComparer();
      string rule = "FREQ=MONTHLY;INTERVAL=3;BYMONTHDAY=20,22;COUNT=5";

      var sut = new RRULE(rule);

      var expected = new List<byte>()
      {
        20,
        22
      };

      Assert.Equal<byte>(expected, sut.ByMonthDay, comparer);
    }

    [Fact]
    public void ParseRule_ThrowWhen_MonthlyHasByDayAndByMonthDay()
    {
      string rule = "FREQ=MONTHLY;INTERVAL=3;BYMONTHDAY=20;BYDAY=3MO;COUNT=5";

      Action act = () => new RRULE(rule);

      Assert.Throws<ArgumentException>(act);
    }

    // 3rd monday of every month until ..
    [Fact]
    public void ParseRule_UntilSomeDate()
    {
      string rule = "FREQ=MONTHLY;BYDAY=3MO;UNTIL=20200301T000000";

      var expected = new DateTime(2020, 3, 1, 0, 0, 0);

      var sut = new RRULE(rule);

      Assert.Equal(expected, sut.Until);
    }

    #endregion
  }
}
