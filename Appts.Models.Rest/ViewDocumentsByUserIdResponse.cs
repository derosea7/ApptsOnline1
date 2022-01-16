using System;
using System.Collections.Generic;
using System.Text;
using Appts.Models.Document;
namespace Appts.Models.Rest
{
  public class ViewDocumentsByUserIdResponse
  {
    public SubscriptionDocument Subscription { get; set; }
    public ServiceProviderDocument ServiceProvider { get; set; }
    public TrialPromotionDocument TrialPromo { get; set; }
    public StripeCustomerDocument StripeCustomer { get; set; }
    public ClientDoc Client { get; set; }
    public List<AvailabilityPeriod> Availability { get; set; }
    public List<AppointmentType> AppointmentTypes{ get; set; }
    public List<Appointment> Appointments{ get; set; }
  }
}
