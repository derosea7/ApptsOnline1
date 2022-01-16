using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace Appts.Models.Document
{
  /// <summary>
  /// Container for general availability timeframe, or period.
  /// </summary>
  public class AvailabilityPeriod : Document
  {
    public AvailabilityPeriod()
    {
      EntityType = "AvailPeriod";
    }
    [JsonProperty(PropertyName = "startDate")]
    public DateTime StartDate { get; set; }
    [JsonProperty(PropertyName = "endDate")]
    public DateTime? EndDate { get; set; }
    [JsonProperty(PropertyName = "availability")]
    public List<Availability> Availability { get; set; }
    /// <summary>
    /// Create a queue of dates from a given range.
    /// </summary>
    /// <param name="start">First date in queue.</param>
    /// <param name="range">Length of queue.</param>
    /// <returns>Queue of dates to be processed</returns>
    private Queue<Availability> CreateDateQueue(DateTime start, int range)
    {
      // create queue of dates to process
      var availabilityQueue = new Queue<Availability>();
      DateTime concreateDate;
      for (int i = 0; i < range; i++)
      {
        concreateDate = start.AddDays(i).Date;
        availabilityQueue.Enqueue(new Availability()
        {
          ConcreteDate = concreateDate,
          DayOfWeek = concreateDate.DayOfWeek.ToString()
        });
      }
      return availabilityQueue;
    }
    /// <summary>
    /// Given a list of periods that contain the availability for a desired range of dates,
    /// combine them into a single Period that can be displayed in the output.
    /// 
    /// Precondition: periods must be pre-filtered to include only those that
    /// should be coalesced (i.e. those that the desired range fall in).
    /// 
    /// Max range is 365.
    /// </summary>
    /// <param name="periods">List of periods that contain availability for desired range</param>
    /// <param name="start">First day of desired range. Range always a full week--7 days.</param>
    /// <param name="range">Length of days to project availability. Max of 365.</param>
    public void CoalescePeriods(List<AvailabilityPeriod> periods, DateTime start, int range = 7)
    {
      if (range > 365)
        throw new ArgumentOutOfRangeException("Range must be 365 or less.");
      // handle null end dates, signifying indefinite period
      foreach (AvailabilityPeriod period in periods)
      {
        if (period.EndDate == null)
          period.EndDate = DateTime.MaxValue;
      }
      Availability = new List<Availability>();
      var availabilityQueue = CreateDateQueue(start, range);
      Availability next;
      while (availabilityQueue.TryDequeue(out next))
      {
        foreach (AvailabilityPeriod period in periods)
        {
          if (DoesDateExistInPeriod((DateTime)next.ConcreteDate, period.StartDate, period.EndDate))
          {
            Availability.Add(new Availability()
            {
              ConcreteDate = next.ConcreteDate,
              DayOfWeek = next.DayOfWeek,
              Blocks = period.Availability.Find(a => a.DayOfWeek == next.DayOfWeek).Blocks
            });

            break;
            //var blocksToUpdate = period.Availability.Find(a => a.DayOfWeek == next.DayOfWeek).Blocks;
            //var doBlocksExist = blocksToUpdate == null ? false : true;
            //if (!doBlocksExist)
            //{ 
            //  Availability.Add(new Availability()
            //  {
            //    ConcreteDate = next.ConcreteDate,
            //    DayOfWeek = next.DayOfWeek,
            //    Blocks = period.Availability.Find(a => a.DayOfWeek == next.DayOfWeek).Blocks
            //  });
            //  // prevents second period on same day from being included
            //  //break;
            //}
            //else
            //{

            //}
          }
        }
      }
      return;
    }
    private bool DoesDateExistInPeriod(DateTime date, DateTime periodStart, DateTime? periodEnd)
    {
      return (periodStart <= date && periodEnd >= date);
    }
    /// <summary>
    /// Given a list of appointments, remove all scheduled blocks,
    /// leaving only availble time blocks in that period.
    /// </summary>
    /// <param name="appointments">List of appointments that will cancel out available time blocks</param>
    public void RemoveAllScheduledBlocks(List<Appointment> appointments)
    {
      foreach (Availability availability in Availability)
      {
        if (availability != null)
        {
          availability.ExpandBlocks();
          // here, must consider both start and end time of appt
          // break appts spanning 2 days up
          //List<Appointment> todaysAppts = appointments.Where(a => a.StartTime.Date == availability.ConcreteDate).ToList();
          List<Appointment> todaysAppts = appointments.Where(a =>
            a.StartTime.Date == availability.ConcreteDate
            || a.EndTime.Date == availability.ConcreteDate).ToList();
          List<Appointment> adjustedAppts = ApplyTypeBuffers(todaysAppts, availability.ConcreteDate);
          availability.RemoveScheduledBlocks(adjustedAppts);
        }
      }
    }
    private Queue<DateTime> CreateAvailabilityTimesQueue(DateTime startTime)
    {
      Queue<DateTime> times = new Queue<DateTime>();
      foreach (Availability availability in Availability)
      {
        if (availability.Blocks != null && availability.ConcreteDate != null)
        {
          foreach (AvailabilityBlock block in availability.Blocks)
          {
            //is block after simulated now? only show those that are now or future
            if (availability.ConcreteDate + block.StartTime >= startTime)
            times.Enqueue((DateTime)availability.ConcreteDate + block.StartTime);
          }
        }
      }
      return times;
    }
    //public void RemoveBlocksInPast(DateTime zonedNow)
    //{

    //}
    private List<ContiguousAvailabilityBlock> CreateContiguousBlocksList(Queue<DateTime> times)
    {
      var contiguousBlocks = new List<ContiguousAvailabilityBlock>();
      DateTime contiguousStart = DateTime.MinValue;
      DateTime contiguousEnd;
      // should parameterize this for customization
      var interval = new TimeSpan(0, 15, 0);
      DateTime current;
      DateTime next;
      bool hasNext;
      bool captureStart = true;
      while (times.TryDequeue(out current))
      {
        if (captureStart)
        {
          contiguousStart = current;
          captureStart = false;
        }
        hasNext = times.TryPeek(out next);
        if (hasNext)
        {
          if (current + interval == next)
          {
            //string contiguous = "";
          }
          else
          {
            contiguousEnd = current + interval;
            //at this point we know the contiguous duration
            //captureStart = true for next contiguoous block
            contiguousBlocks.Add(new ContiguousAvailabilityBlock(contiguousStart, contiguousEnd));
            captureStart = true;
          }
        }
        else
        {
          contiguousEnd = current + interval;
          /// evaluate lack block for duration
          contiguousBlocks.Add(new ContiguousAvailabilityBlock(contiguousStart, contiguousEnd));
        }
      }
      return contiguousBlocks;
    }
    // given an appt ttype, leave only the blocks that can fit an appt
    // bug, not removing blocks before gap that does not leave enought time to finsih appt
    public void RemoveBlocksWithoutEnoughTime(AppointmentType appointmentType, DateTime startTime)
    {
      //create list of datetimes
      // curent block is first block
      // find end of contiguous availability block
      // if there is enough time between for appt type, then move on
      // if there is not enough time, remove blocks 
      Queue<DateTime> times = CreateAvailabilityTimesQueue(startTime);
      var contiguousBlocks = CreateContiguousBlocksList(times);
      //determine which cblocks to remove
      var cBlocksToKeep = new List<ContiguousAvailabilityBlock>();
      foreach (ContiguousAvailabilityBlock contiguousBlock in contiguousBlocks)
      {
        if (appointmentType.Duration <= contiguousBlock.EndDate - contiguousBlock.StartDate)
        {
          cBlocksToKeep.Add(contiguousBlock);
        }
      }
      var aBlocksToKeep = new List<AvailabilityBlock>();
      var availabilityToKeep = new List<Availability>();
      foreach (Availability availability in Availability)
      {
        foreach (ContiguousAvailabilityBlock cBlock in cBlocksToKeep)
        {
          string s = cBlock.StartDate.ToString();
          if (availability.ConcreteDate == cBlock.StartDate.Date || availability.ConcreteDate == cBlock.EndDate.Date)
          {
            if (availability.Blocks != null)
            {
              string ss = availability.ConcreteDate.ToString();
              foreach (var availabilityBlock in availability.Blocks)
              {
                if (availability.ConcreteDate + availabilityBlock.EndTime > cBlock.EndDate)
                {
                  break;
                }
                DateTime aBlockStart = (DateTime)availability.ConcreteDate + availabilityBlock.StartTime;
                DateTime aBlockEnd = (DateTime)availability.ConcreteDate + availabilityBlock.EndTime;
                if (aBlockStart >= cBlock.StartDate && aBlockEnd <= cBlock.EndDate &&
                  cBlock.EndDate - aBlockStart >= appointmentType.Duration)
                {
                  //keep cblock
                  aBlocksToKeep.Add(availabilityBlock);
                }
                else { }
              }
            }
          }
        }
        availabilityToKeep.Add(new Availability()
        {
          Blocks = aBlocksToKeep,
          ConcreteDate = availability.ConcreteDate
        });
        aBlocksToKeep = new List<AvailabilityBlock>(); // restart blocks to keep
      }
      //update orignal blocks
      UpdateAvailabilityBlocks(availabilityToKeep);
      //foreach (Availability originalA in Availability)
      //{
      //  foreach (Availability newA in availabilityToKeep)
      //  {
      //    if (originalA.ConcreteDate == newA.ConcreteDate)
      //    {
      //      originalA.Blocks = newA.Blocks;
      //    }
      //    else if (newA.ConcreteDate > originalA.ConcreteDate)
      //    {
      //      break; //note: depends on availability lists being sorted small to large
      //    }
      //  }
      //}
    }
    void UpdateAvailabilityBlocks(List<Availability> updated)
    {
      foreach (Availability originalA in Availability)
      {
        foreach (Availability newA in updated)
        {
          if (originalA.ConcreteDate == newA.ConcreteDate)
          {
            originalA.Blocks = newA.Blocks;
          }
          else if (newA.ConcreteDate > originalA.ConcreteDate)
          {
            break; //note: depends on availability lists being sorted small to large
          }
        }
      }
    }
    public List<Appointment> ApplyTypeBuffers(List<Appointment> todaysAppointments, DateTime? concreteDate)
    {
      List<Appointment> adjustedAppts = new List<Appointment>();
      // will also try handlnig start/end buffer here
      // if not, splitting by day then adding buffer causes error

      // adjust start and end time
      // if an appt starts today but ends tomorrow, set end time = end of date
      // not, only handling case when appt is 24 hours or less
      // so 1 appt can span a max of 2 days
      TimeSpan endOfDay = new TimeSpan(0, 23, 59, 0);
      foreach (Appointment appt in todaysAppointments)
      {
        if (appt.StartTime.Date != appt.EndTime.Date)
        {
          if (appt.StartTime.Date != concreteDate)
          {
            appt.StartTime = appt.StartTime.Date + TimeSpan.Zero;
            // buffer end time only
            if (appt.AppointmentType?.BufferAfter != null)
            {
              appt.EndTime = appt.EndTime.Date + appt.EndTime.TimeOfDay.Add(appt.AppointmentType.BufferAfter);
            }
          }
          else
          {
            // copying appointment object here to avoid bug with updating reference
            // TODO: improve code
            Appointment appt2 = new Appointment();
            appt2.EndTime = appt.EndTime.Date + endOfDay;
            // buff start time only
            if (appt.AppointmentType?.BufferBefore != null)
            {
              appt2.StartTime = appt.StartTime.Date + appt.StartTime.TimeOfDay.Add(appt.AppointmentType.BufferBefore.Negate());
            }
            adjustedAppts.Add(appt2);
          }
        }
        else
        {
          // adjust both start and end for buffer
          if (appt.AppointmentType?.BufferBefore != null)
          {
            appt.StartTime = appt.StartTime.Date + appt.StartTime.TimeOfDay.Add(appt.AppointmentType.BufferBefore.Negate());
          }
          if (appt.AppointmentType?.BufferAfter != null)
          {
            appt.EndTime = appt.EndTime.Date + appt.EndTime.TimeOfDay.Add(appt.AppointmentType.BufferAfter);
          }
        }
        adjustedAppts.Add(appt);
      }
      return adjustedAppts;
    }
  }
}
