using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.Domain
{
  public class OutgoingEmail
  {
    public string To { get; set; }
    public string From { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    /// <summary>
    /// Specific to SendGrid.
    /// </summary>
    public string TemplateId { get; set; }
  }
}
