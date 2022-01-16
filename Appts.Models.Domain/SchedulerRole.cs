using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Domain
{
  public enum SchedulerRole
  {
    Undefined,
    AnonymousScheduler,
    Client,
    Subscriber,
    FreeSubscriber,
    SelfScheduler
  }
}
