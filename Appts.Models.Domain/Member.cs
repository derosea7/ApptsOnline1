using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Domain
{
  public class Member : User
  {
    public List<OrganizationRole> Roles { get; set; }
  }
}
