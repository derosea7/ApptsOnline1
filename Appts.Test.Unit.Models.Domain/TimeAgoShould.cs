using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Appts.Models.Document;
namespace Appts.Test.Unit.Models.Domain
{
  public class TimeAgoShould
  {
    [Fact]
    public void ShowDateTimeFrom60SecondsAgoAs_1HourAgo()
    {
      DateTime startDate = new DateTime(2021, 2, 5, 20, 35, 0);
      DateTime inputDate = new DateTime(2021, 2, 5, 21, 35, 0);
      string sut = TimeAgo.Calculate(inputDate, startDate);

      Assert.Equal("an hour ago", sut);
    }
  }
}
