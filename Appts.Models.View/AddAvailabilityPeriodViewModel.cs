using Appts.Models.Api;
using Appts.Models.Document;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Appts.Models.View
{
  public class AddAvailabilityPeriodViewModel
  {
    // get
    public List<PeriodDatesApiModel> PeriodDates { get; set; }
    public bool Active { get; set; } = true;
    // used on an edit--know which period is being edittedeed.
    public string PeriodId { get; set; }

    // used when running javascript, validating new period dates
    public string PeriodDatesJson { get; set; }
    // store json in hidden input until required on front end

    // post
    [Required(ErrorMessage = "Must select a start date")]
    public DateTime? PeriodStart { get; set; }
    [Required(ErrorMessage = "Must select an end date")]
    public DateTime? PeriodEnd { get; set; }
    public List<Availability> AvailabilityDays { get; set; }

    // used when loading existing availability on a GET
    // javascript can pick this up and print out on UI
    public string AvailabilityDaysJson { get; set; }

    public string PeriodJson { get; set; }

    // true when loading from invalid post, or performing edit
    public bool IsEditScenario { get; set; }
  }
}
