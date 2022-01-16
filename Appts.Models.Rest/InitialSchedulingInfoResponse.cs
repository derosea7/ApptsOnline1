using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Document;
using Appts.Models.Domain;

namespace Appts.Models.Rest
{
  public class InitialSchedulingInfoResponse
  {
    public ServiceProviderDocument ServiceProvider { get; set; }
    public ClientDoc ClientInfo { get; set; }
    //Used when scheduler is client, will display list of clients sp can select
    public List<ClientDoc> ClientList { get; set; }
    public List<AppointmentType> AppointmentTypes { get; set; }
    public SchedulerRole SchedulerRole { get; set; }
    //public string AvailabilityTimeZoneId { get; set; }
  }
}
