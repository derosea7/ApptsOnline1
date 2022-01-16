using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Document;
namespace Appts.Models.Rest
{
  public class SaveApptRequest
  {
    //the appt to save
    public Appointment Appointment { get; set; }
    //false if user is anonymous and therefor no relationship to create
    public bool CreateRelationshipIfNotExists { get; set; }

    //used in email confirmation, actual url
    public string SpVanityUrl { get; set; }
    //just the vanity name, not full url
    public string SpVanity { get; set; }
    //build cancel / reschedule link for emails / text with this
    public string SiteBaseUrl { get; set; }
  }
}
