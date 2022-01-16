using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Appts.Models.Domain;
using Appts.Models.Api;
using Appts.Models.Document;

namespace Appts.Models.View
{
  public class ScheduleAppointmentViewModel
  {
    //public Business Business { get; set; }

    //// no longer using select lst
    ////public List<SelectListItem> AppointmentTypes { get; set; }

    public string AppointmentTypeId { get; set; }
    public Appointment Appointment { get; set; }
    public UserRole UserRole { get; set; }
    public SchedulerRole SchedulerRole { get; set; }

    ////public List<AppointmentType> AppointmentTypes2 { get; set; }
    public List<AppointmentType> AppointmentTypes2 { get; set; }

    //// true if appt exists and requires reschedule flow
    //public bool Reschedule { get; set; } = false;

    ////// if false, can display something in UI indicating that
    ////// appt may be incorrect
    ////public bool FoundAppointmentToReschedule { get; set; }

    //// if this is null, then appt not found. can remove prop above
    //public Appointment AppointmentToReschedule { get; set; }

    // if true, menas that this GET is a result of the client 
    // attempting to post with invalid data state.
    // UI will need to be set with what information the user
    // had initially set
    public bool IsGetFromInvalidPost { get; set; }



    // view model fields.. cannot figure out how to seperate
    // binding for view model, while enabling validation
    public string ServiceProviderId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string LocationDetails { get; set; }
    public string Location { get; set; }
    public string Notes { get; set; }
    //public string AppointmentTypeId { get; set; }
    public string AppointmentTypeBreif { get; set; }
    public string BusinessTimeZone { get; set; }

    public string ClientId { get; set; }

    //[Required]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string ClientFirstName { get; set; }

    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string ClientLastName { get; set; }

    [EmailAddress]
    [Display(Name = "Email")]
    public string ClientEmail { get; set; }

    public string ClientTimeZone { get; set; }

    public string ClientMobilePhone { get; set; }
    public string ClientAddress { get; set; }
    public string ClientAddress2 { get; set; }
    public string ClientCity { get; set; }
    public string ClientState { get; set; }
    public string ClientStateCode { get; set; }
    public string ClientZipCode { get; set; }

    public List<ClientDoc> ClientList { get; set; }
  

    // attributes that need to be carried from initial
    // get to post, so that if model state is invalid
    // we can call the GET again with enough info
    // to get the user back to where they left off
    // before leaving invalid input
    public string BusinessDisplayName { get; set; }




    public bool Reschedule { get; set; }
    public TimeSpan AppointmentDuration { get; set; }
    public string AppointmentIdToReschedule { get; set; }

    public string SpVanityUrl { get; set; }
  }
}
