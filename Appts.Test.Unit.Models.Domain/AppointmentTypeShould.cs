using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  public class AppointmentTypeShould
  {
    [Fact]
    public void ShowReadableSummaryOnToString_MinutesOnly()
    {
      var sut = new AppointmentType()
      {
        Name = "Programming Tutoring",
        Duration = TimeSpan.FromMinutes(45)
      };

      string summary = sut.ToString();

      Assert.Equal("Programming Tutoring | 45 mins", summary);
    }

    [Fact]
    public void ShowReadableSummaryOnToString_1DigitHour_1DigitMin()
    {
      var sut = new AppointmentType()
      {
        Name = "Programming Tutoring",
        Duration = TimeSpan.FromMinutes(65)
      };

      string summary = sut.ToString();

      Assert.Equal("Programming Tutoring | 1 hr 5 mins", summary);
    }

    [Fact]
    public void ShowPluralHours_2DigitHour_2DigitMin()
    {
      var sut = new AppointmentType()
      {
        Name = "Programming Tutoring",
        Duration = TimeSpan.FromMinutes(125)
      };

      string summary = sut.ToString();

      Assert.Equal("Programming Tutoring | 2 hrs 5 mins", summary);
    }
  }
}
