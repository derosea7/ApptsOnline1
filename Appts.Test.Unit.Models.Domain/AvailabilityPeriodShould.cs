using System;
using Xunit;
using System.Collections.Generic;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  public class AvailabilityPeriodShould
  {

    //// 10/10/2019, invalid test after refactoring to use queue + break when
    //// reached end of desired range
    //[Fact]
    //public void PopulateConcreteDateRangeWhenCoalescing_1WeekFromStartDate()
    //{
    //  var sut = new AvailabilityPeriod();
    //  var periods = new List<AvailabilityPeriod>()
    //  {
    //    new AvailabilityPeriod() { },
    //    new AvailabilityPeriod() { }
    //  };
    //  var startRangeDate = new DateTime(2019, 9, 8);
    //  sut.CoalescePeriods(periods, startRangeDate);

    //  var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
    //  var monday = sut.Availability.Find(a => a.DayOfWeek == "Monday");

    //  Assert.Equal(startRangeDate, sunday.ConcreteDate);
    //  Assert.Equal(startRangeDate.AddDays(1), monday.ConcreteDate);
    //}

    /// <summary>
    /// September 8 - 11 (sun - wed) should come from period 1
    /// September 12 - 14 (thur - sat) should come from period 2
    /// </summary>
    [Fact]
    public void Coalesce2PeriodsForClientDisplay()
    {
      // arrange
      AvailabilityPeriod sut = new AvailabilityPeriod();
      var periods = Get2SuccessivePeriods();
      var startRangeDate = new DateTime(2019, 9, 8);
      var vacayBlocks = GetAvailabilityBlocks_Vacation();
      var workBlocks = GetAvailabilityBlocks_WorkWeek();
      //var comparer = new CompareBlockLists();
      var comparer = new AvailabilityBlockComparer();
      
      // act
      sut.CoalescePeriods(periods, startRangeDate);

      // assert
      var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
      var monday = sut.Availability.Find(a => a.DayOfWeek == "Monday");
      var tuesday = sut.Availability.Find(a => a.DayOfWeek == "Tuesday");
      var wednesday = sut.Availability.Find(a => a.DayOfWeek == "Wednesday");
      var thursday = sut.Availability.Find(a => a.DayOfWeek == "Thursday");
      var friday = sut.Availability.Find(a => a.DayOfWeek == "Friday");
      var saturday = sut.Availability.Find(a => a.DayOfWeek == "Saturday");

      Assert.Equal<AvailabilityBlock>(vacayBlocks, sunday.Blocks, comparer);
      Assert.Equal<AvailabilityBlock>(vacayBlocks, monday.Blocks, comparer);
      Assert.Equal<AvailabilityBlock>(vacayBlocks, tuesday.Blocks, comparer);
      Assert.Equal<AvailabilityBlock>(vacayBlocks, wednesday.Blocks, comparer);

      // go back to work :(
      Assert.Equal<AvailabilityBlock>(workBlocks, thursday.Blocks, comparer);
      Assert.Equal<AvailabilityBlock>(workBlocks, friday.Blocks, comparer);

      Assert.Equal<AvailabilityBlock>(vacayBlocks, saturday.Blocks, comparer);
    }
    /// <summary>
    /// 10/10/2019, coalesce should handle more than a week at a time.
    /// September 8 - 11 (sun - wed) should come from period 1
    /// September 12 - 14 (thur - sat) should come from period 2
    /// </summary>
    [Fact]
    public void Coalesce2PeriodsFor_ARangeGreaterThan7Days()
    {
      // arrange
      AvailabilityPeriod sut = new AvailabilityPeriod();
      var periods = Get2SuccessivePeriods();
      var startRangeDate = new DateTime(2019, 9, 8);
      var vacayBlocks = GetAvailabilityBlocks_Vacation();
      var workBlocks = GetAvailabilityBlocks_WorkWeek();
      int amountOfDaysInFinalPeriod = 210;

      // expectations
      var comparer = new AvailabilityBlockComparer();
      var expectedMonday1 = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(10, 0, 0), new TimeSpan(11, 30, 0)),
        new AvailabilityBlock(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0)),
        new AvailabilityBlock(new TimeSpan(17, 25, 0), new TimeSpan(23, 30, 0))
      };
      var expectedMonday2 = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(18, 30, 0), new TimeSpan(23, 0, 0))
      };
      var expectedMonday3 = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(18, 30, 0), new TimeSpan(23, 0, 0))
      };

      // act
      sut.CoalescePeriods(periods, startRangeDate, amountOfDaysInFinalPeriod);

      // assert
      var monday1 = sut.Availability.Find(a => a.ConcreteDate == new DateTime(2019, 9, 9));
      var monday2 = sut.Availability.Find(a => a.ConcreteDate == new DateTime(2019, 9, 16));
      var monday3 = sut.Availability.Find(a => a.ConcreteDate == new DateTime(2019, 9, 23));
      
      Assert.Equal<AvailabilityBlock>(expectedMonday1, monday1.Blocks, comparer);
      Assert.Equal<AvailabilityBlock>(expectedMonday2, monday2.Blocks, comparer);
      Assert.Equal<AvailabilityBlock>(expectedMonday3, monday3.Blocks, comparer);

    }
    [Fact]
    public void Coalesce1PeriodForClientDisplay()
    {
      var sut = new AvailabilityPeriod();
      var startRangeDate = new DateTime(2019, 9, 8);
      var vacayBlocks = GetAvailabilityBlocks_Vacation();
      List<AvailabilityPeriod> singlePeriod = GetPeriod_SundayAvailabilityOnly(vacayBlocks);
      var comparer = new AvailabilityBlockComparer();

      // act
      sut.CoalescePeriods(singlePeriod, startRangeDate);

      // assert
      var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
      var monday = sut.Availability.Find(a => a.DayOfWeek == "Monday");
      var tuesday = sut.Availability.Find(a => a.DayOfWeek == "Tuesday");
      var wednesday = sut.Availability.Find(a => a.DayOfWeek == "Wednesday");
      //var thursday = sut.Availability.Find(a => a.DayOfWeek == "Thursday");
      //var friday = sut.Availability.Find(a => a.DayOfWeek == "Friday");
      //var saturday = sut.Availability.Find(a => a.DayOfWeek == "Saturday");

      Assert.Equal<AvailabilityBlock>(vacayBlocks, sunday.Blocks, comparer);
      Assert.Null(monday.Blocks);
      Assert.Null(tuesday.Blocks);
      Assert.Null(wednesday.Blocks);
      //Assert.Null(thursday.Blocks);
      //Assert.Null(friday.Blocks);
      //Assert.Null(saturday.Blocks);
    }

    /// <summary>
    /// Should not throw error when there are no appoitnments to remove.
    /// </summary>
    [Fact]
    public void ExpandBlocksAndExcludeAppointments_NullApptsList()
    {
      var sut = new AvailabilityPeriod();
      var sundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(10, 30, 0), new TimeSpan(11, 15, 0))
      };
      List<AvailabilityPeriod> singlePeriod = GetPeriod_SundayAvailabilityOnly(sundayBlocks);
      var startRangeDate = new DateTime(2019, 9, 8);
      var comparer = new AvailabilityBlockComparer();
      var apptDate = new DateTime(2019, 9, 8);

      sut.CoalescePeriods(singlePeriod, startRangeDate);
      var appts = new List<Appointment>();
      var expectedBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(10, 30, 0), new TimeSpan(10, 45, 0)),
        new AvailabilityBlock(new TimeSpan(10, 45, 0), new TimeSpan(11, 0, 0)),
        new AvailabilityBlock(new TimeSpan(11, 0, 0), new TimeSpan(11, 15, 0)),
      };
      
      // act
      sut.RemoveAllScheduledBlocks(appts);

      // assert
      var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
      Assert.Equal<AvailabilityBlock>(expectedBlocks, sunday.Blocks, comparer);
    }

    [Fact]
    public void ExpandBlocksAndExcludeAppointments()
    {
      var sut = new AvailabilityPeriod();
      var sundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(9, 30, 0), new TimeSpan(11, 30, 0))
      };
      List<AvailabilityPeriod> singlePeriod = GetPeriod_SundayAvailabilityOnly(sundayBlocks);
      var startRangeDate = new DateTime(2019, 9, 8);
      var comparer = new AvailabilityBlockComparer();
      var apptDate = new DateTime(2019, 9, 8);
      
      sut.CoalescePeriods(singlePeriod, startRangeDate);
      var appts = new List<Appointment>()
      {
        new Appointment()
        {
          StartTime = apptDate + new TimeSpan(10, 0, 0),
          EndTime = apptDate + new TimeSpan(10, 30, 0),
          AppointmentType = new AppointmentType()
          {
            BufferBefore = new TimeSpan(0, 5, 0),
            BufferAfter = new TimeSpan(0, 10, 0)
          }
        },
        new Appointment()
        {
          StartTime = apptDate + new TimeSpan(11, 0, 0),
          EndTime = apptDate + new TimeSpan(11, 15, 0)
        }
      };

      var expectedBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(9, 30, 0), new TimeSpan(9, 45, 0)),
        new AvailabilityBlock(new TimeSpan(10, 45, 0), new TimeSpan(11, 0, 0)),
        new AvailabilityBlock(new TimeSpan(11, 15, 0), new TimeSpan(11, 30, 0)),
      };

      // act
      sut.RemoveAllScheduledBlocks(appts);

      // assert
      var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
      Assert.Equal<AvailabilityBlock>(expectedBlocks, sunday.Blocks, comparer);
    }

    [Fact]
    public void ExpandBlocksAndExcludeAppointments_ApptSpans2Days()
    {
      var sut = new AvailabilityPeriod();
      var sundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(22, 30, 0), new TimeSpan(23, 45, 0))
      };
      var mondayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(0, 0, 0), new TimeSpan(2, 45, 0))
      };

      var overnightAvailable = new AvailabilityPeriod()
      {
        StartDate = new DateTime(2019, 9, 1),
        EndDate = new DateTime(2019, 9, 11),
        Availability = new List<Availability>()
        {
          GetAvailability("Sunday", sundayBlocks),
          GetAvailability("Monday", mondayBlocks),
          GetAvailability("Tuesday"),
          GetAvailability("Wednesday"),
          GetAvailability("Thursday"),
          GetAvailability("Friday"),
          GetAvailability("Saturday")
        }
      };
      //AvailabilityPeriod p = 
      List<AvailabilityPeriod> singlePeriod = new List<AvailabilityPeriod>() { overnightAvailable };

      var startRangeDate = new DateTime(2019, 9, 8);
      var comparer = new AvailabilityBlockComparer();

      var apptStartDate = new DateTime(2019, 9, 8);
      var apptEndDate = new DateTime(2019, 9, 9);

      sut.CoalescePeriods(singlePeriod, startRangeDate);
      var appts = new List<Appointment>()
      {
        new Appointment()
        {
          StartTime = apptStartDate + new TimeSpan(23, 15, 0),
          EndTime = apptEndDate + new TimeSpan(1, 30, 0),
          AppointmentType = new AppointmentType()
          {
            BufferBefore = new TimeSpan(0, 5, 0),
            BufferAfter = new TimeSpan(0, 10, 0)
          }
        },
        new Appointment()
        {
          StartTime = apptStartDate + new TimeSpan(11, 0, 0),
          EndTime = apptStartDate + new TimeSpan(11, 15, 0)
        }
      };

      var expectedSundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(22, 30, 0), new TimeSpan(22, 45, 0)),
        new AvailabilityBlock(new TimeSpan(22, 45, 0), new TimeSpan(23, 0, 0))
      };

      var expectedMondayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(1, 45, 0), new TimeSpan(2, 0, 0)),
        new AvailabilityBlock(new TimeSpan(2, 0, 0), new TimeSpan(2, 15, 0)),
        new AvailabilityBlock(new TimeSpan(2, 15, 0), new TimeSpan(2, 30, 0)),
        new AvailabilityBlock(new TimeSpan(2, 30, 0), new TimeSpan(2, 45, 0))
      };



      // act
      sut.RemoveAllScheduledBlocks(appts);

      // assert
      var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
      var monday = sut.Availability.Find(a => a.DayOfWeek == "Monday");

      Assert.Equal<AvailabilityBlock>(expectedSundayBlocks, sunday.Blocks, comparer);
      Assert.Equal<AvailabilityBlock>(expectedMondayBlocks, monday.Blocks, comparer);
    }

    //avail start = 8a, first app start = 9a, 1 hour diff
    [Fact]
    public void OnlyScheduleApptIfEnoughTime_FirstApptStartAfterAvailStart_OneDay()
    {
      var sut = new AvailabilityPeriod();
      var sundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(8, 0, 0), new TimeSpan(12, 45, 0))
      };
      var ap = new AvailabilityPeriod()
      {
        StartDate = new DateTime(2021, 2, 7),
        EndDate = new DateTime(2021, 2, 7),
        Availability = new List<Availability>()
        {
          GetAvailability("Sunday", sundayBlocks),
          GetAvailability("Monday"),
          GetAvailability("Tuesday"),
          GetAvailability("Wednesday"),
          GetAvailability("Thursday"),
          GetAvailability("Friday"),
          GetAvailability("Saturday")
        }
      };
      var periods = new List<AvailabilityPeriod>() { ap };
      var apptType = new AppointmentType()
      {
        Name = "test",
        Duration = new TimeSpan(0, 45, 0),
        IsActive = true
      };
      var apptStartDate = new DateTime(2021, 2, 7);
      var appts = new List<Appointment>()
      {
        new Appointment()
        {
          StartTime = apptStartDate + new TimeSpan(9, 0, 0),
          EndTime = apptStartDate + new TimeSpan(9, 30, 0)
        },
        new Appointment()
        {
          StartTime = apptStartDate + new TimeSpan(10, 0, 0),
          EndTime = apptStartDate + new TimeSpan(10, 30, 0)
        },
        new Appointment()
        {
          StartTime = apptStartDate + new TimeSpan(11, 15, 0),
          EndTime = apptStartDate + new TimeSpan(11, 45, 0)
        }
      };
      var startRangeDate = new DateTime(2021, 2, 7);
      var simulatedNow = new DateTime(2021, 2, 7, 0, 0, 0);
      sut.CoalescePeriods(periods, startRangeDate);
      sut.RemoveAllScheduledBlocks(appts);
      sut.RemoveBlocksWithoutEnoughTime(apptType, simulatedNow);

      var expectedSundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(8, 0, 0), new TimeSpan(8, 15, 0)),
        new AvailabilityBlock(new TimeSpan(8, 15, 0), new TimeSpan(8, 30, 0)),
        new AvailabilityBlock(new TimeSpan(10, 30, 0), new TimeSpan(10, 45, 0)),
        new AvailabilityBlock(new TimeSpan(11, 45, 0), new TimeSpan(12, 0, 0)),
        new AvailabilityBlock(new TimeSpan(12, 0, 0), new TimeSpan(12, 15, 0))
      };

      var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
      var comparer = new AvailabilityBlockComparer();
      Assert.Equal<AvailabilityBlock>(expectedSundayBlocks, sunday.Blocks, comparer);
    }
    //fixes bug on scheduling screen where availability blocks are shown
    //before now (past availability cannot be scheduled)
    [Fact]
    public void ShouldEliminateTimes_BeforeZonedNow()
    {
      var sut = new AvailabilityPeriod();
      var sundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(7, 30, 0), new TimeSpan(12, 0, 0))
      };
      var ap = new AvailabilityPeriod()
      {
        StartDate = new DateTime(2021, 2, 7),
        EndDate = new DateTime(2021, 2, 7),
        Availability = new List<Availability>()
        {
          GetAvailability("Sunday", sundayBlocks),
          GetAvailability("Monday"),
          GetAvailability("Tuesday"),
          GetAvailability("Wednesday"),
          GetAvailability("Thursday"),
          GetAvailability("Friday"),
          GetAvailability("Saturday")
        }
      };
      var periods = new List<AvailabilityPeriod>() { ap };
      var apptType = new AppointmentType()
      {
        Name = "test",
        Duration = new TimeSpan(0, 15, 0),
        IsActive = true
      };
      var apptStartDate = new DateTime(2021, 2, 7);
      var appts = new List<Appointment>(){ };
      var startRangeDate = new DateTime(2021, 2, 7);
      //should not show times before thiss
      var simulatedNow = new DateTime(2021, 2, 7, 10, 0, 0);
      sut.CoalescePeriods(periods, startRangeDate);
      sut.RemoveAllScheduledBlocks(appts);
      sut.RemoveBlocksWithoutEnoughTime(apptType, simulatedNow);
      //sut.RemovedBlocksInPast(simulatedNow);
      var expectedSundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(10, 0, 0), new TimeSpan(10, 15, 0)),
        new AvailabilityBlock(new TimeSpan(10, 15, 0), new TimeSpan(10, 30, 0)),
        new AvailabilityBlock(new TimeSpan(10, 30, 0), new TimeSpan(10, 45, 0)),
        new AvailabilityBlock(new TimeSpan(10, 45, 0), new TimeSpan(11, 0, 0)),
        new AvailabilityBlock(new TimeSpan(11, 0, 0), new TimeSpan(11, 15, 0)),
        new AvailabilityBlock(new TimeSpan(11, 15, 0), new TimeSpan(11, 30, 0)),
        new AvailabilityBlock(new TimeSpan(11, 30, 0), new TimeSpan(11, 45, 0)),
        new AvailabilityBlock(new TimeSpan(11, 45, 0), new TimeSpan(12, 0, 0))
      };
      var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
      var comparer = new AvailabilityBlockComparer();
      Assert.Equal<AvailabilityBlock>(expectedSundayBlocks, sunday.Blocks, comparer);
    }
    [Fact]
    public void OnlyScheduleApptIfEnoughTime_FirstApptAtAvailStartTime_OneDay()
    {
      var sut = new AvailabilityPeriod();
      var sundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(9, 0, 0), new TimeSpan(11, 15, 0))
      };
      var ap = new AvailabilityPeriod()
      {
        StartDate = new DateTime(2021, 2, 7),
        EndDate = new DateTime(2021, 2, 7),
        Availability = new List<Availability>()
        {
          GetAvailability("Sunday", sundayBlocks),
          GetAvailability("Monday"),
          GetAvailability("Tuesday"),
          GetAvailability("Wednesday"),
          GetAvailability("Thursday"),
          GetAvailability("Friday"),
          GetAvailability("Saturday")
        }
      };
      var periods = new List<AvailabilityPeriod>() { ap };
      var apptType = new AppointmentType()
      {
        Name = "test",
        Duration = new TimeSpan(0, 45, 0),
        IsActive = true
      };
      var apptStartDate = new DateTime(2021, 2, 7);
      var appts = new List<Appointment>()
      {
        new Appointment()
        {
          StartTime = apptStartDate + new TimeSpan(9, 0, 0),
          EndTime = apptStartDate + new TimeSpan(9, 30, 0)
        },
        new Appointment()
        {
          StartTime = apptStartDate + new TimeSpan(10, 0, 0),
          EndTime = apptStartDate + new TimeSpan(10, 30, 0)
        }
      };
      var startRangeDate = new DateTime(2021, 2, 7);
      var simulatedNow = new DateTime(2021, 2, 7, 0, 0, 0);
      sut.CoalescePeriods(periods, startRangeDate);
      sut.RemoveAllScheduledBlocks(appts);
      sut.RemoveBlocksWithoutEnoughTime(apptType, simulatedNow);
      var expectedSundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(10, 30, 0), new TimeSpan(10, 45, 0))
      };
      var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
      var comparer = new AvailabilityBlockComparer();
      Assert.Equal<AvailabilityBlock>(expectedSundayBlocks, sunday.Blocks, comparer);
    }
    [Fact]
    public void AllowOneOffPeriods()
    {
      var sut = new AvailabilityPeriod();
      var sundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(7, 30, 0), new TimeSpan(12, 0, 0))
      };
      var ap1 = new AvailabilityPeriod()
      {
        StartDate = new DateTime(2021, 2, 7),
        EndDate = new DateTime(2021, 2, 7),
        Availability = new List<Availability>()
        {
          GetAvailability("Sunday", sundayBlocks),
          GetAvailability("Monday"),
          GetAvailability("Tuesday"),
          GetAvailability("Wednesday"),
          GetAvailability("Thursday"),
          GetAvailability("Friday"),
          GetAvailability("Saturday")
        }
      };
      var periods = new List<AvailabilityPeriod>() { ap1 };
      var oneoffSundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(15, 0, 0), new TimeSpan(16, 15, 0))
      };
      var apOneoff = new AvailabilityPeriod()
      {
        StartDate = new DateTime(2021, 2, 7),
        EndDate = new DateTime(2021, 2, 7),
        Availability = new List<Availability>()
        {
          GetAvailability("Sunday", oneoffSundayBlocks),
          GetAvailability("Monday"),
          GetAvailability("Tuesday"),
          GetAvailability("Wednesday"),
          GetAvailability("Thursday"),
          GetAvailability("Friday"),
          GetAvailability("Saturday")
        }
      };
      //var oneoffPeriod = new List<AvailabilityPeriod>() { apOneoff };
      periods.Add(apOneoff);

      var apptType = new AppointmentType()
      {
        Name = "test",
        Duration = new TimeSpan(0, 15, 0),
        IsActive = true
      };
      var apptStartDate = new DateTime(2021, 2, 7);
      var appts = new List<Appointment>() { };
      var startRangeDate = new DateTime(2021, 2, 7);
      //should not show times before thiss
      var simulatedNow = new DateTime(2021, 2, 7, 10, 0, 0);
      sut.CoalescePeriods(periods, startRangeDate);
      sut.RemoveAllScheduledBlocks(appts);
      sut.RemoveBlocksWithoutEnoughTime(apptType, simulatedNow);
      //sut.RemovedBlocksInPast(simulatedNow);
      var expectedSundayBlocks = new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(10, 0, 0), new TimeSpan(10, 15, 0)),
        new AvailabilityBlock(new TimeSpan(10, 15, 0), new TimeSpan(10, 30, 0)),
        new AvailabilityBlock(new TimeSpan(10, 30, 0), new TimeSpan(10, 45, 0)),
        new AvailabilityBlock(new TimeSpan(10, 45, 0), new TimeSpan(11, 0, 0)),
        new AvailabilityBlock(new TimeSpan(11, 0, 0), new TimeSpan(11, 15, 0)),
        new AvailabilityBlock(new TimeSpan(11, 15, 0), new TimeSpan(11, 30, 0)),
        new AvailabilityBlock(new TimeSpan(11, 30, 0), new TimeSpan(11, 45, 0)),
        new AvailabilityBlock(new TimeSpan(11, 45, 0), new TimeSpan(12, 0, 0)),
        //one off blocks
        new AvailabilityBlock(new TimeSpan(15, 0, 0), new TimeSpan(15, 15, 0)),
        new AvailabilityBlock(new TimeSpan(15, 15, 0), new TimeSpan(15, 30, 0)),
        new AvailabilityBlock(new TimeSpan(15, 30, 0), new TimeSpan(15, 45, 0)),
        new AvailabilityBlock(new TimeSpan(15, 45, 0), new TimeSpan(16, 0, 0)),
        new AvailabilityBlock(new TimeSpan(16, 0, 0), new TimeSpan(16, 15, 0))
      };
      var sunday = sut.Availability.Find(a => a.DayOfWeek == "Sunday");
      var comparer = new AvailabilityBlockComparer();
      Assert.Equal<AvailabilityBlock>(expectedSundayBlocks, sunday.Blocks, comparer);
    }
    #region "Helpers"

    private List<AvailabilityPeriod> GetPeriod_SundayAvailabilityOnly(List<AvailabilityBlock> sundayBlocks)
    {
      var vacayPeriod = new AvailabilityPeriod()
      {
        StartDate = new DateTime(2019, 9, 1),
        EndDate = new DateTime(2019, 9, 11),
        Availability = new List<Availability>()
        {
          GetAvailability("Sunday", sundayBlocks),
          GetAvailability("Monday"),
          GetAvailability("Tuesday"),
          GetAvailability("Wednesday"),
          GetAvailability("Thursday"),
          GetAvailability("Friday"),
          GetAvailability("Saturday")
        }
      };

      return new List<AvailabilityPeriod>() { vacayPeriod };
    }

    private List<AvailabilityPeriod> Get2SuccessivePeriods()
    {
      var vacationBlocks = GetAvailabilityBlocks_Vacation();
      var workBlocks = GetAvailabilityBlocks_WorkWeek();

      var vacation = new AvailabilityPeriod()
      {
        StartDate = new DateTime(2019, 9, 1),
        EndDate = new DateTime(2019, 9, 11),
        Availability = new List<Availability>()
      };
      vacation.Availability.Add(GetAvailability("Sunday", vacationBlocks));
      vacation.Availability.Add(GetAvailability("Monday", vacationBlocks));
      vacation.Availability.Add(GetAvailability("Tuesday", vacationBlocks));
      vacation.Availability.Add(GetAvailability("Wednesday", vacationBlocks));
      vacation.Availability.Add(GetAvailability("Thursday", vacationBlocks));
      vacation.Availability.Add(GetAvailability("Friday", vacationBlocks));
      vacation.Availability.Add(GetAvailability("Saturday", vacationBlocks));

      var work = new AvailabilityPeriod()
      {
        StartDate = new DateTime(2019, 9, 12),
        EndDate = new DateTime(2020, 4, 1),
        Availability = new List<Availability>()
      };
      work.Availability.Add(GetAvailability("Sunday"));
      work.Availability.Add(GetAvailability("Monday", workBlocks));
      work.Availability.Add(GetAvailability("Tuesday", workBlocks));
      work.Availability.Add(GetAvailability("Wednesday", workBlocks));
      work.Availability.Add(GetAvailability("Thursday", workBlocks));
      work.Availability.Add(GetAvailability("Friday", workBlocks));
      work.Availability.Add(GetAvailability("Saturday", vacationBlocks));

      return new List<AvailabilityPeriod>()
      {
        vacation,
        work
      };
    }
    private List<AvailabilityBlock> GetAvailabilityBlocks_Vacation()
    {
      return new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(10, 0, 0), new TimeSpan(11, 30, 0)),
        new AvailabilityBlock(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0)),
        new AvailabilityBlock(new TimeSpan(17, 25, 0), new TimeSpan(23, 30, 0))
      };
    }

    private List<AvailabilityBlock> GetAvailabilityBlocks_Vacation_Variant()
    {
      return new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(10, 0, 0), new TimeSpan(11, 30, 0)),
        new AvailabilityBlock(new TimeSpan(13, 0, 0), new TimeSpan(17, 0, 0)),
        new AvailabilityBlock(new TimeSpan(17, 25, 0), new TimeSpan(23, 00, 0))
      };
    }

    private Availability GetAvailability(
      string availabilityDay,
      List<AvailabilityBlock> blocks = null)
    {
      return new Availability()
      {
        DayOfWeek = availabilityDay,
        Blocks = blocks
      };
    }

    private List<AvailabilityBlock> GetAvailabilityBlocks_WorkWeek()
    {
      return new List<AvailabilityBlock>()
      {
        new AvailabilityBlock(new TimeSpan(18, 30, 0), new TimeSpan(23, 0, 0))
      };
    }


    #endregion

  }

}
