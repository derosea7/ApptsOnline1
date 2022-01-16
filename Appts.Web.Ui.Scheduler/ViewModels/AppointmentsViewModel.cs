using Appts.Models.View;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class AppointmentsViewModel
  {
    public List<AppointmentsByDateViewModel> AppointmentByDate { get; set; }
    public string AppointmentTypeId { get; set; }
    public List<SelectListItem> AppointmentTypes { get; set; }

    public string AppointmentStatus { get; set; }
    public List<SelectListItem> AppointmentStatusOptions { get; set; }



    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // appts will be converted to this time zone
    public string DisplayIanaTimeZoneId { get; set; }
    public string SpVanityUrl { get; set; }

    //public DateTime? StartDate { get; set; }
    //public DateTime? EndDate { get; set; }
    public int NewApptCount { get; set; }
    public List<string> NewApptIds { get; set; }
  }
}
