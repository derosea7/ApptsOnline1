using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Appts.Models.Document;

namespace Appts.Models.View
{
  public class ManageClientsViewModel
  {
    // get
    public string ServiceProviderVanityUrl { get; set; }
    //public string ServiceProviderName { get; set; }
    public string ServiceProviderDisplayName { get; set; }
    public string SpEmail { get; set; }
    public List<ClientDoc> Clients { get; set; }

    //test 12/4/2019
    //for vanity url construction
    public string RequestHost { get; set; }

    // POST
    [EmailAddress]
    public string Email { get; set; }
    [Phone]
    public string Phone { get; set; }
    
    [StringLength(180)]
    public string PersonalMessage { get; set; }

    public bool SendSms { get; set; }
    public bool SendEmail { get; set; }
  }
}
