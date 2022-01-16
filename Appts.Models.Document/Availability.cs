using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  /// <summary>
  /// Represents a block of time that providers can offer their services.
  /// </summary>
  public class Availability// : Document
  {
    //[JsonProperty(PropertyName = "isReOccurring")]
    //public bool IsReoccurring { get; set; }

    [JsonProperty(PropertyName = "dayOfWeek")]
    public string DayOfWeek { get; set; }

    /// <summary>
    /// Only used when re-displaying back to client with
    /// concrete dates. When the Availability is used
    /// as a generic day, this will be null.
    /// </summary>
    [JsonProperty(PropertyName = "concreteDate")]
    public DateTime? ConcreteDate { get; set; }

    //[JsonProperty(PropertyName = "sequence")]
    //public byte Sequence { get; set; }

    [JsonProperty(PropertyName = "blocks")]
    public List<AvailabilityBlock> Blocks { get; set; }

    /// <summary>
    /// True when blocks have been expanded and no longer represent a 
    /// series of time.
    /// </summary>
    [JsonIgnore]
    public bool BlocksAreExpanded { get; set; }

    /// <summary>
    /// Transform the representation of a block of availability to
    /// discrete intervals of availability. These intervals will 
    /// be displayed to clients when scheduling appointments.
    /// </summary>
    public void ExpandBlocks()
    {
      List<AvailabilityBlock> original = Blocks;
      var expanded = new List<AvailabilityBlock>();

      // should parameterize this for customization
      var interval = new TimeSpan(0, 15, 0);

      if (original == null) return;

      foreach (AvailabilityBlock block in original)
      {
        if (block != null)
        {
          var timeCounter = block.StartTime;
          while ((timeCounter + interval) <= block.EndTime)
          {
            expanded.Add(new AvailabilityBlock(timeCounter, timeCounter + interval));

            timeCounter += interval;
          }
        }
      }//foreach


      BlocksAreExpanded = true;
      Blocks = expanded;
    }

    /// <summary>
    /// Updates the reference, adjusting start time by subtracting (negating) buffer before,
    /// and end time by adding buffer after.
    /// </summary>
    /// <param name="appointments">The list of appointments to be updated</param>
    private void ApplyAppointmentTypeBuffers(List<Appointment> appointments)
    {
      Appointment appt;

      for (int a = 0; a < appointments.Count; a++)
      {
        appt = appointments[a];

        if (appt.AppointmentType?.BufferBefore != null)
        {
          appt.StartTime = appt.StartTime.Date + appt.StartTime.TimeOfDay.Add(appt.AppointmentType.BufferBefore.Negate());
        }
        //else
        //{
        //  appt.StartTime = appt.StartTime.Date + appt.StartTime.TimeOfDay;
        //}

        if (appt.AppointmentType?.BufferAfter != null)
        {
          appt.EndTime = appt.EndTime.Date + appt.EndTime.TimeOfDay.Add(appt.AppointmentType.BufferAfter);
        }
        //else
        //{
        //  appt.EndTime = appt.EndTime.Date + appt.EndTime.TimeOfDay;
        //}

        a++;
      }
    }

    // TODO: passing types as seperate list here because of flaw in approach
    // to designing appt model. 
    public void RemoveScheduledBlocks(List<Appointment> appointments)
    {
      var originalBlocks = Blocks;
      if (originalBlocks == null) return;

      var updatedBlocks = new List<AvailabilityBlock>();
      bool keepBlock;

      //ApplyAppointmentTypeBuffers(appointments);

      // for each available block
      foreach (AvailabilityBlock block in originalBlocks)
      {
        if (block != null)
        {
          keepBlock = true;
          foreach (Appointment appt in appointments)
          {
            if (ApptInterruptsBlock(appt.StartTime.TimeOfDay, appt.EndTime.TimeOfDay, block))
            {
              keepBlock = false;
              break;
            }
          }

          if (keepBlock == true)
            updatedBlocks.Add(block);
        }
      }//foreach block

      Blocks = updatedBlocks;
    }

    private bool ApptInterruptsBlock(TimeSpan apptStart, TimeSpan apptEnd, AvailabilityBlock block)
    {
      if (
        ApptStartsWithOrDuringBlock(apptStart, block) ||
        ApptEncompassesBlock(apptStart, apptEnd, block) ||
        ApptEndsWithOrDuringBlock(apptEnd, block))
        return true;
      else
        return false;
    }

    private bool ApptEncompassesBlock(TimeSpan apptStart, TimeSpan apptEnd, AvailabilityBlock block)
    {
      if (apptStart < block.StartTime && apptEnd > block.EndTime)
        return true;
      else
        return false;
    }

    private bool ApptEndsWithOrDuringBlock(TimeSpan apptEnd, AvailabilityBlock block)
    {
      if (apptEnd == block.EndTime || TimeOccursDuringBlock(apptEnd, block))
        return true;
      else
        return false;
    }

    private bool ApptStartsWithOrDuringBlock(TimeSpan apptStart, AvailabilityBlock block)
    {
      if (apptStart == block.StartTime || TimeOccursDuringBlock(apptStart, block))
        return true;
      else
        return false;
    }

    private bool TimeOccursDuringBlock(TimeSpan time, AvailabilityBlock block)
    {
      if (time > block.StartTime && time < block.EndTime)
        return true;
      else
        return false;
    }

    public override string ToString()
    {
      return $"{DayOfWeek} | {ConcreteDate?.ToShortDateString()} | {Blocks.Count}";
    }
  }
}
