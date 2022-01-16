using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class AddClientVm
  {
    public string ClientUserId { get; set; }
    public bool IsOnboarding { get; set; }
    public string TimeZoneId { get; set; }
    [Phone]
    public string MobilePhone { get; set; }
    [StringLength(128)]
    public string Address { get; set; }
    [StringLength(128)]
    public string Address2 { get; set; }
    [StringLength(32)]
    public string City { get; set; }
    [StringLength(2)]
    public string StateCode { get; set; }
    public List<SelectListItem> StateOptions { get; set; }
    [StringLength(10)]
    public string ZipCode { get; set; }
    //public string Country { get; set; }
  }
}
