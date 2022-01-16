using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  public class RecurringAppointmentShould
  {
    #region "DAILY"

    [Fact]
    public void CreateApptListFromRRULE_EveryOtherDay_UntilNov5()
    {
      var comparer = new AppointmentComparer();
      string rule = "FREQ=DAILY;INTERVAL=2;UNTIL=20191105T150000";
      var originalAppt = new Appointment()
      {
        StartTime = new DateTime(2019, 10, 20, 15, 0, 0),
        EndTime = new DateTime(2019, 10, 20, 15, 45, 0),
        AppointmentTypeId = "sometype",
        RRULE = rule
      };

      var expected = new List<Appointment>()
      {
        new Appointment(
          new DateTime(2019, 10, 20, 15, 0, 0),
          new DateTime(2019, 10, 20, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 10, 22, 15, 0, 0),
          new DateTime(2019, 10, 22, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 10, 24, 15, 0, 0),
          new DateTime(2019, 10, 24, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 10, 26, 15, 0, 0),
          new DateTime(2019, 10, 26, 15, 45, 0)),

        new Appointment(
          new DateTime(2019, 10, 28, 15, 0, 0),
          new DateTime(2019, 10, 28, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 10, 30, 15, 0, 0),
          new DateTime(2019, 10, 30, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 11, 1, 15, 0, 0),
          new DateTime(2019, 11, 1, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 11, 3, 15, 0, 0),
          new DateTime(2019, 11, 3, 15, 45, 0)),

        new Appointment(
          new DateTime(2019, 11, 5, 15, 0, 0),
          new DateTime(2019, 11, 5, 15, 45, 0))
      };

      List<Appointment> sut = RecurringAppointment.GetAppointments(originalAppt);

      Assert.Equal<Appointment>(expected, sut, comparer);
    }

    #endregion

    #region "WEEKLY"

    // note: week start always SU (WKST=SU)
    [Fact]
    public void CreateApptListFromRRULE_Every3WeeksOnMonAndWed_3Times()
    {
      var comparer = new AppointmentComparer();
      string rule = "FREQ=WEEKLY;INTERVAL=3;BYDAY=MO,WE;COUNT=3";

      var originalAppt = new Appointment()
      {
        StartTime = new DateTime(2019, 10, 21, 15, 0, 0),
        EndTime = new DateTime(2019, 10, 21, 15, 45, 0),
        AppointmentTypeId = "sometype",
        RRULE = rule
      };

      var expected = new List<Appointment>()
      {
        new Appointment(
          new DateTime(2019, 10, 21, 15, 0, 0),
          new DateTime(2019, 10, 21, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 10, 23, 15, 0, 0),
          new DateTime(2019, 10, 23, 15, 45, 0)),

        new Appointment(
          new DateTime(2019, 11, 11, 15, 0, 0),
          new DateTime(2019, 11, 11, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 11, 13, 15, 0, 0),
          new DateTime(2019, 11, 13, 15, 45, 0)),

        new Appointment(
          new DateTime(2019, 12, 2, 15, 0, 0),
          new DateTime(2019, 12, 2, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 12, 4, 15, 0, 0),
          new DateTime(2019, 12, 4, 15, 45, 0))
      };

      List<Appointment> sut = RecurringAppointment.GetAppointments(originalAppt);

      Assert.Equal<Appointment>(expected, sut, comparer);
    }

    [Fact]
    public void CreateApptListFromRRULE_Every5WeeksOnSunAndSat_3Times()
    {
      var comparer = new AppointmentComparer();
      string rule = "FREQ=WEEKLY;INTERVAL=2;BYDAY=SU,SA;UNTIL=20191231T235959";

      var originalAppt = new Appointment()
      {
        StartTime = new DateTime(2019, 10, 20, 15, 0, 0),
        EndTime = new DateTime(2019, 10, 20, 15, 45, 0),
        AppointmentTypeId = "sometype",
        RRULE = rule
      };

      var expected = new List<Appointment>()
      {
        new Appointment(
          new DateTime(2019, 10, 20, 15, 0, 0),
          new DateTime(2019, 10, 20, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 10, 26, 15, 0, 0),
          new DateTime(2019, 10, 26, 15, 45, 0)),

        new Appointment(
          new DateTime(2019, 11, 3, 15, 0, 0),
          new DateTime(2019, 11, 3, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 11, 9, 15, 0, 0),
          new DateTime(2019, 11, 9, 15, 45, 0)),

        new Appointment(
          new DateTime(2019, 11, 17, 15, 0, 0),
          new DateTime(2019, 11, 17, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 11, 23, 15, 0, 0),
          new DateTime(2019, 11, 23, 15, 45, 0)),

        new Appointment(
          new DateTime(2019, 12, 1, 15, 0, 0),
          new DateTime(2019, 12, 1, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 12, 7, 15, 0, 0),
          new DateTime(2019, 12, 7, 15, 45, 0)),

        new Appointment(
          new DateTime(2019, 12, 15, 15, 0, 0),
          new DateTime(2019, 12, 15, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 12, 21, 15, 0, 0),
          new DateTime(2019, 12, 21, 15, 45, 0)),

        new Appointment(
          new DateTime(2019, 12, 29, 15, 0, 0),
          new DateTime(2019, 12, 29, 15, 45, 0))
      };

      List<Appointment> sut = RecurringAppointment.GetAppointments(originalAppt);

      Assert.Equal<Appointment>(expected, sut, comparer);
    }

    #endregion

    #region "MONTHLY"

    [Fact]
    public void CreateApptListFromRRULE_Every3MonthsOn20thOfMonth_3Times()
    {
      var comparer = new AppointmentComparer();

      string rule = "FREQ=MONTHLY;INTERVAL=3;BYMONTHDAY=20;COUNT=3";
      var originalAppt = new Appointment()
      {
        StartTime = new DateTime(2019, 10, 20, 15, 0, 0),
        EndTime = new DateTime(2019, 10, 20, 15, 45, 0),
        AppointmentTypeId = "sometype",
        RRULE = rule
      };

      var expected = new List<Appointment>()
      {
        new Appointment()
        {
          StartTime = new DateTime(2019, 10, 20, 15, 0, 0),
          EndTime = new DateTime(2019, 10, 20, 15, 45, 0)
        },
        new Appointment()
        {
          StartTime = new DateTime(2020, 1, 20, 15, 0, 0),
          EndTime = new DateTime(2020, 1, 20, 15, 45, 0)
        },
        new Appointment()
        {
          StartTime = new DateTime(2020, 4, 20, 15, 0, 0),
          EndTime = new DateTime(2020, 4, 20, 15, 45, 0)
        }
      };

      List<Appointment> sut = RecurringAppointment.GetAppointments(originalAppt);

      Assert.Equal<Appointment>(expected, sut, comparer);
    }

    [Fact]
    public void CreateApptListFromRRULE_Every2MonthsOn20thOfMonth_UntilMay2020()
    {
      var comparer = new AppointmentComparer();
      string rule = "FREQ=MONTHLY;INTERVAL=2;BYMONTHDAY=20;UNTIL=20200501T000000";
      var originalAppt = new Appointment()
      {
        StartTime = new DateTime(2019, 10, 20, 15, 0, 0),
        EndTime = new DateTime(2019, 10, 20, 15, 45, 0),
        AppointmentTypeId = "sometype",
        RRULE = rule
      };

      var expected = new List<Appointment>()
      {
        new Appointment()
        {
          StartTime = new DateTime(2019, 10, 20, 15, 0, 0),
          EndTime = new DateTime(2019, 10, 20, 15, 45, 0)
        },
        new Appointment()
        {
          StartTime = new DateTime(2019, 12, 20, 15, 0, 0),
          EndTime = new DateTime(2019, 12, 20, 15, 45, 0)
        },
        new Appointment()
        {
          StartTime = new DateTime(2020, 2, 20, 15, 0, 0),
          EndTime = new DateTime(2020, 2, 20, 15, 45, 0)
        },
        new Appointment()
        {
          StartTime = new DateTime(2020, 4, 20, 15, 0, 0),
          EndTime = new DateTime(2020, 4, 20, 15, 45, 0)
        }
      };

      List<Appointment> sut = RecurringAppointment.GetAppointments(originalAppt);

      Assert.Equal<Appointment>(expected, sut, comparer);
    }

    [Fact]
    public void CreateApptListFromRRULE_Every2MonthsOn20thAnd22ndOfMonth_UntilMay2020()
    {
      var comparer = new AppointmentComparer();
      string rule = "FREQ=MONTHLY;INTERVAL=2;BYMONTHDAY=20,22;UNTIL=20200501T000000";
      var originalAppt = new Appointment()
      {
        StartTime = new DateTime(2019, 10, 20, 15, 0, 0),
        EndTime = new DateTime(2019, 10, 20, 15, 45, 0),
        AppointmentTypeId = "sometype",
        RRULE = rule
      };

      var expected = new List<Appointment>()
      {
        new Appointment("10/20/2019 3:00 PM", 45),
        new Appointment("10/22/2019 3:00 PM", 45),
        new Appointment("12/20/2019 3:00 PM", 45),
        new Appointment("12/22/2019 3:00 PM", 45),

        new Appointment("2/20/2020 3:00 PM", 45),
        new Appointment("2/22/2020 3:00 PM", 45),
        new Appointment("4/20/2020 3:00 PM", 45),
        new Appointment("4/22/2020 3:00 PM", 45)
      };

      List<Appointment> sut = RecurringAppointment.GetAppointments(originalAppt);

      Assert.Equal<Appointment>(expected, sut, comparer);
    }

    [Fact]
    public void CreateApptListFromRRULE_Every2MonthsOn3rdMonday_UntilMay2020()
    {
      var comparer = new AppointmentComparer();
      string rule = "FREQ=MONTHLY;INTERVAL=2;BYDAY=3MO;UNTIL=20200501T000000";
      var originalAppt = new Appointment()
      {
        StartTime = new DateTime(2019, 10, 21, 15, 0, 0),
        EndTime = new DateTime(2019, 10, 21, 15, 45, 0),
        AppointmentTypeId = "sometype",
        RRULE = rule
      };

      var expected = new List<Appointment>()
      {
        new Appointment()
        {
          StartTime = new DateTime(2019, 10, 21, 15, 0, 0),
          EndTime = new DateTime(2019, 10, 21, 15, 45, 0)
        },
        new Appointment()
        {
          StartTime = new DateTime(2019, 12, 16, 15, 0, 0),
          EndTime = new DateTime(2019, 12, 16, 15, 45, 0)
        },
        new Appointment()
        {
          StartTime = new DateTime(2020, 2, 17, 15, 0, 0),
          EndTime = new DateTime(2020, 2, 17, 15, 45, 0)
        },
        new Appointment()
        {
          StartTime = new DateTime(2020, 4, 20, 15, 0, 0),
          EndTime = new DateTime(2020, 4, 20, 15, 45, 0)
        }
      };

      List<Appointment> sut = RecurringAppointment.GetAppointments(originalAppt);

      Assert.Equal<Appointment>(expected, sut, comparer);
    }

    [Fact]
    public void CreateApptListFromRRULE_Every2MonthsOn3rdMonday_5Times()
    {
      var comparer = new AppointmentComparer();
      string rule = "FREQ=MONTHLY;INTERVAL=2;BYDAY=3MO;COUNT=5";
      var originalAppt = new Appointment()
      {
        StartTime = new DateTime(2019, 10, 21, 15, 0, 0),
        EndTime = new DateTime(2019, 10, 21, 15, 45, 0),
        AppointmentTypeId = "sometype",
        RRULE = rule
      };

      var expected = new List<Appointment>()
      {
        new Appointment(
          new DateTime(2019, 10, 21, 15, 0, 0),
          new DateTime(2019, 10, 21, 15, 45, 0)),
        new Appointment(
          new DateTime(2019, 12, 16, 15, 0, 0),
          new DateTime(2019, 12, 16, 15, 45, 0)),
        new Appointment(
          new DateTime(2020, 2, 17, 15, 0, 0),
          new DateTime(2020, 2, 17, 15, 45, 0)),
        new Appointment(
          new DateTime(2020, 4, 20, 15, 0, 0),
          new DateTime(2020, 4, 20, 15, 45, 0)),
        new Appointment(
          new DateTime(2020, 6, 15, 15, 0, 0),
          new DateTime(2020, 6, 15, 15, 45, 0))
      };

      List<Appointment> sut = RecurringAppointment.GetAppointments(originalAppt);

      Assert.Equal<Appointment>(expected, sut, comparer);
    }

    #endregion

    //[Fact]
    //public void Get_20thOfNextMonth()
    //{
    //  var now = DateTime.Now.Date;

    //  DateTime next20th = new DateTime(now.Year, now.AddMonths(1).Month, 20);

    //  Assert.Equal(new DateTime(2019, 11, 20), next20th);
    //}
  }
}
