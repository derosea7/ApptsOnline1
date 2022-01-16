using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Models.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class ApptsCalendarViewModel
  {
    public string CalendarDataHash { get; set; }
    public string CalendarEvents { get; set; }
    public UserRole UserRole { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string DisplayIanaTimeZoneId { get; set; }
    public string SpVanityUrl { get; set; }
    public string AppointmentTypeId { get; set; }
    public List<SelectListItem> AppointmentTypes { get; set; }
    public string AppointmentStatus { get; set; }
    public List<SelectListItem> AppointmentStatusOptions { get; set; }

  }
}
