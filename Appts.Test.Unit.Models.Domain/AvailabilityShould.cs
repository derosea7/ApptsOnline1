using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  public class AvailabilityShould
  {
    [Fact]
    public void ExpandBlocksBy15MinuteIntervals()
    {
      var sut = new Availability()
      {
        Blocks = new List<AvailabilityBlock>()
        {
          new AvailabilityBlock(new TimeSpan(8, 0, 0), new TimeSpan(9, 15, 0)),
          new AvailabilityBlock(new TimeSpan(12, 30, 0), new TimeSpan(14, 0, 0))
        }
      };

      sut.ExpandBlocks();

      Assert.Equal(new TimeSpan(8, 0, 0), sut.Blocks[0].StartTime);
      Assert.Equal(new TimeSpan(8, 15, 0), sut.Blocks[1].StartTime);
      Assert.Equal(new TimeSpan(9, 0, 0), sut.Blocks[4].StartTime);
      Assert.Equal(new TimeSpan(12, 30, 0), sut.Blocks[5].StartTime);

      Assert.True(sut.BlocksAreExpanded);
    }
    [Fact]
    public void RemoveBlocksConcurrentWithAppointment_ApptStartsAndEndsWithBlock()
    {
      var comparer = new AvailabilityBlockComparer();
      var dateOfAppt = new DateTime(2019, 9, 6);
      var apptStart = new TimeSpan(8, 0, 0);
      var apptEnd = new TimeSpan(9, 0, 0);
      var sut = new Availability()
      {
        Blocks = new List<AvailabilityBlock>()
        {
          new AvailabilityBlock(new TimeSpan(8, 0, 0), new TimeSpan(10, 15, 0)),
        }
      };
      sut.ExpandBlocks();

      var appt1 = new Appointment()
      {
        StartTime = dateOfAppt + apptStart,
        EndTime = dateOfAppt + apptEnd
      };

      List<Appointment> appts = new List<Appointment>()
      {
        appt1
      };
      var scheduledBlock = new AvailabilityBlock(new TimeSpan(8, 15, 0), new TimeSpan(8, 30, 0));


      // act
      sut.RemoveScheduledBlocks(appts);

      // assert
      Assert.DoesNotContain<AvailabilityBlock>(scheduledBlock, sut.Blocks, comparer);
    }

    [Fact]
    public void RemoveBlocksConcurrentWithAppointment_ApptStartsInMiddleOfBlock()
    {
      var comparer = new AvailabilityBlockComparer();
      var dateOfAppt = new DateTime(2019, 9, 6);

      // starts in middle of available block
      var apptStart = new TimeSpan(8, 20, 0);

      var apptEnd = new TimeSpan(9, 0, 0);
      var sut = new Availability()
      {
        Blocks = new List<AvailabilityBlock>()
        {
          new AvailabilityBlock(new TimeSpan(8, 0, 0), new TimeSpan(10, 15, 0)),
        }
      };
      sut.ExpandBlocks();

      var appt1 = new Appointment()
      {
        StartTime = dateOfAppt + apptStart,
        EndTime = dateOfAppt + apptEnd
      };

      List<Appointment> appts = new List<Appointment>()
      {
        appt1
      };
      var scheduledBlock = new AvailabilityBlock(new TimeSpan(8, 15, 0), new TimeSpan(8, 30, 0));

      // act
      sut.RemoveScheduledBlocks(appts);

      // assert
      Assert.DoesNotContain<AvailabilityBlock>(scheduledBlock, sut.Blocks, comparer);
    }
  }
}