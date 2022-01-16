using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class DeleteAppointmentTypeRequest
  {
    public string AppointmentTypeId { get; set; }
    public string ServiceProviderId { get; set; }
  }
}
