using Appts.Models.Document;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.Models
{
  public static class Extensions
  {
    public static List<SelectListItem> ToSelectList(this List<AppointmentType> appointmentTypes)
    {
      var apptTypesOptions = new List<SelectListItem>();
      apptTypesOptions.Add(new SelectListItem() { Text = "Choose...", Value = "" });
      foreach (AppointmentType type in appointmentTypes)
      {
        string displayText = $"{type.Name} [";
        if (type.Duration.Hours > 0)
        {
          displayText += $"{type.Duration:hh} Hour(s) ";
        }
        displayText += $"{type.Duration:mm} Minutes]";

        apptTypesOptions.Add(new SelectListItem(displayText, type.Id));
      }

      return apptTypesOptions;
      //return new List<SelectListItem>();
    }
  }
}
