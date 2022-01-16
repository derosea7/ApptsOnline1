using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Domain
{
  public enum UserRole
  {
    Undefined,
    AnonymousScheduler,
    Client,
    Subscriber,
    FreeSubscriber,
    SelfScheduler
  }
}
