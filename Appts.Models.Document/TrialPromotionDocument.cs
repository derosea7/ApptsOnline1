using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Document
{
  public class TrialPromotionDocument : Document
  {
    public double ttl { get; set; }

    // sequence no returned from service bus when
    // scheduling a message that will eventually trigger an email
    // use these to cancel emails if needed
    public List<long> ScheduledEmailMessageSequenceNumbers { get; set; }

    public TrialPromotionDocument()
    {
      EntityType = "TrialPromo";
    }
  }
}
