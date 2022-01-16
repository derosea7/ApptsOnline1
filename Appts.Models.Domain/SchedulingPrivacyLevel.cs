using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Domain
{
  public enum SchedulingPrivacyLevel
  {
    Undefined,

    // not authenticated
    AllowAnonymous,

    // authenticated
    AllowAnyRegistered,

    // authenticated and email approved by sp
    AllowWhitelisted
  }
}
