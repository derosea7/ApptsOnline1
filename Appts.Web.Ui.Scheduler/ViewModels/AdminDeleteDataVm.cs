using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Appts.Web.Ui.Scheduler.ViewModels
{
  public class AdminDeleteDataVm
  {
    //get
    [StringLength(50)]
    [Display(Name = "User Id")]
    public string UserId { get; set; }
    public string EntityType { get; set; }
    //warning; wipes data by user id
    public bool DeleteAllEntities { get; set; }

    //post
    public int DeletedCount { get; set; }
  }
}
