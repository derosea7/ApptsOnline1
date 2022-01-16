using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Appts.Models.Domain;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  public class AppointmentShould
  {
    [Fact]
    public void ShowReadableSummaryOnToString_StartEndBreif()
    {
      Appointment sut = new Appointment()
      {
        StartTime = DateTime.Now.AddHours(2),
        EndTime = DateTime.Now.AddHours(3),
        AppointmentTypeBreif = "Some breife explanation of type"
      };

      string summary = sut.ToString();
    }

    [Fact]
    public void CreateDateTimeInSelectedTimeZone()
    {
      Appointment sut = new Appointment()
      {
        StartTime = DateTime.Parse("2019-10-12T11:45:00"),
        EndTime = DateTime.Now.AddHours(3),
        AppointmentTypeBreif = "Some breife explanation of type"
      };

      DateTime start = sut.StartTime;
      string s = start.ToString("o");
    }
  }
}
