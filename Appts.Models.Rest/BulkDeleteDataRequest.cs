using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class BulkDeleteDataRequest
  {
    public string EntityType { get; set; }
    public string UserId { get; set; }
    //warning; wipes all data
    public bool DeleteAllEntities { get; set; }
  }
}
