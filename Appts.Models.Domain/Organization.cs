using System;
using System.Collections.Generic;

namespace Appts.Models.Domain
{
  public class Organization
  {
    public string Name { get; set; }

    /// <summary>
    /// Subscription for the organization.
    /// </summary>
    public Subscription Subscription { get; set; }

    /// <summary>
    /// An organization must have an owner, the main administratoe.
    /// </summary>
    public User Owner { get; set; }

    /// <summary>
    /// Users associated to this organization
    /// </summary>
    //public List<User> Users { get; set; }
    public List<Member> OrganizationMembers { get; set; }
  }
}
