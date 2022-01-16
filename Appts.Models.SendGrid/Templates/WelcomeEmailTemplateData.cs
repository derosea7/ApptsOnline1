using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Models.SendGrid.Templates
{
  public class WelcomeEmailTemplateData : IDynamicTemplateData
  {
    public string FirstName { get; set; }
  }
}
