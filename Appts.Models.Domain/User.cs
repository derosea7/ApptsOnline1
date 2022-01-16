using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Domain
{
  public class User
  {
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public List<string> Emails { get; set; }
  }
}
