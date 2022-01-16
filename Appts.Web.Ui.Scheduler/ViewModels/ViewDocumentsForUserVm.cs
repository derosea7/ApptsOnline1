using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class ViewDocumentsForUserVm
  {
    public string SubscriptionJson { get; set; }
    public string ServiceProviderJson { get; set; }
    public string TrialPromoJson { get; set; }
    public string StripeCustomerJson { get; set; }
    public string ClientJson { get; set; }
    public List<string> AvailabilityJson { get; set; }
    public List<string> AppointmentTypesJson { get; set; }
    public List<string> AppointmentsJson { get; set; }
  }
}
