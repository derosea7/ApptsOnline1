using Appts.Models.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class ViewAppointmentsRequest
  {
    public string IanaTimeZoneId { get; set; }
    public string UserId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    // optional filter
    public string ApptTypeId { get; set; }
    // optional filter
    public string ApptStatus { get; set; }
    //determines repositories used in query
    public UserRole? UserRole { get; set; }
  }
}
