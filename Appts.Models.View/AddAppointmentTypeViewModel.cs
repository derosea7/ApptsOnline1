using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Appts.Models.View
{
  public class AddAppointmentTypeViewModel : IValidatableObject
  {
    //[StringLength(128)]
    //public string ApptTypeId { get; set; }

    //[StringLength(128)]
    //public string ServiceProviderId { get; set; }
    /// <summary>
    /// When true, record will be made available to public.
    /// </summary>
    [JsonProperty(PropertyName = "isActive")]
    public bool IsActive { get; set; }

    [JsonProperty(PropertyName = "name")]
    [Required]
    [StringLength(128)]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "durationHours")]
    [Range(0, 24)]
    [Display(Name = "Duration Hours")]
    public int? DurationHours { get; set; }

    [JsonProperty(PropertyName = "durationMinutes")]
    [Range(0, 60)]
    [Display(Name = "Duration Minutes")]
    public int? DurationMinutes { get; set; }

    [JsonProperty(PropertyName = "description")]
    public string Description { get; set; }

    //public bool Public { get; set;} // display on main booking page

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    // location details
    public string LocationDetails_WebConf { get; set; }
    public string LocationDetails_CustomerCalls { get; set; }
    public string LocationDetails_WeSpecify { get; set; }

    #region "Cancelation"

    // minimum time required
    [Display(Name = "Cancelation Days")]
    [JsonProperty(PropertyName = "cancelationNoticeDays")]
    [Range(0, 360)]
    public int? CancelationNoticeDays { get; set; }

    [Display(Name = "Cancelation Hours")]
    [JsonProperty(PropertyName = "cancelationNoticeHours")]
    [Range(0, 24)]
    public int? CancelationNoticeHours { get; set; }

    [JsonProperty(PropertyName = "cancelationNoticeMinutes")]
    [Range(0, 60)]
    [Display(Name = "Cancelation Minutes")]
    public int? CancelationNoticeMinutes { get; set; }

    #endregion

    #region "Reschedule Notice"

    // minimum time required
    [JsonProperty(PropertyName = "rescheduleNoticeDays")]
    [Range(0, 365)]
    [Display(Name = "Reschedule Notice Days")]
    public int? RescheduleNoticeDays { get; set; }

    [JsonProperty(PropertyName = "rescheduleNoticeHours")]
    [Range(0, 24)]
    [Display(Name = "Reschedule Notice Hours")]
    public int? RescheduleNoticeHours { get; set; }

    [JsonProperty(PropertyName = "rescheduleNoticeMinutes")]
    [Range(0, 60)]
    [Display(Name = "Reschedule Notice Minutes")]
    public int? RescheduleNoticeMinutes { get; set; }

    #endregion

    #region "Minimum Notice"

    // minimum time required
    [JsonProperty(PropertyName = "minimumNoticeDays")]
    [Range(0, 365)]
    [Display(Name = "Minimum Notice Days")]
    public int? MinimumNoticeDays { get; set; }

    [JsonProperty(PropertyName = "minimumNoticeHours")]
    [Range(0, 24)]
    [Display(Name = "Minimum Notice Hours")]
    public int? MinimumNoticeHours { get; set; }

    [JsonProperty(PropertyName = "minimumNoticeMinutes")]
    [Range(0, 60)]
    [Display(Name = "Minimum Notice Minutes")]
    public int? MinimumNoticeMinutes { get; set; }

    #endregion

    #region "Maximum Notice"

    // maximum time required
    [JsonProperty(PropertyName = "maximumNoticeDays")]
    [Range(0, 365)]
    [Display(Name = "Maximum Notice Days")]
    public int? MaximumNoticeDays { get; set; }

    [JsonProperty(PropertyName = "maximumNoticeHours")]
    [Range(0, 24)]
    [Display(Name = "Maximum Notice Hours")]
    public int? MaximumNoticeHours { get; set; }

    [JsonProperty(PropertyName = "maximumNoticeMinutes")]
    [Range(0, 60)]
    [Display(Name = "Maximum Notice Minutes")]
    public int? MaximumNoticeMinutes { get; set; }

    #endregion

    //// minimum time notice required to schedule
    //[JsonProperty(PropertyName = "requiredNoticeDays")]
    //[Range(0, 365)]
    //[Display(Name = "Required Notice Days")]
    //public int? RequiredNoticeDays { get; set; }

    //[JsonProperty(PropertyName = "rescheduleNoticeMinutes")]
    //[Range(0, 60)]
    //[Display(Name = "Reschedule Minutes")]
    //public int? RescheduleNoticeMinutes { get; set; }

    // prep time
    [JsonProperty(PropertyName = "bufferBeforeHours")]
    [Range(0, 60)]
    [Display(Name = "Buffer Before Hours")]
    public int? BufferBeforeHours { get; set; }

    [JsonProperty(PropertyName = "bufferBeforeMinutes")]
    [Range(0, 60)]
    [Display(Name = "Buffer Before Minutes")]
    public int? BufferBeforeMinutes { get; set; }

    // time allowed after appt before next
    [JsonProperty(PropertyName = "bufferAfterHours")]
    [Range(0, 60)]
    [Display(Name = "Buffer After Hours")]
    public int? BufferAfterHours { get; set; }

    [JsonProperty(PropertyName = "bufferAfterMinutes")]
    [Range(0, 60)]
    [Display(Name = "Buffer After Minutes")]
    public int? BufferAfterMinutes { get; set; }

    //get trues when the record will be editted
    [JsonIgnore]
    public bool IsEditScenario { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
      if (DurationHours == null && DurationMinutes == null)
      {
        yield return new ValidationResult("Duration Hours or Duration Minutes must be populated");
      }
    }
  }
}
