using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Rest
{
  public class SaveApptResponse
  {
    //will be used on the subsequent GET to display the saved details
    public string NewlySavedApptId { get; set; }
  }
}
