using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Appts.Models.Document;

namespace Appts.Test.Unit.Models.Domain
{
  public class ServiceProviderDocumentShould
  {
    [Fact]
    public void CreateUrlFriendlyVanityUrl()
    {
      var sp = new ServiceProviderDocument()
      {
        VanityUrl = "sp live atom $ &"
      };

      sp.CleanVanityUrl();

      Assert.Equal("spliveatom", sp.VanityUrl);
    }

    [Fact]
    public void ReturnFalseWhenValidatingVanityUrl_NotAlphaNumber()
    {
      var sp = new ServiceProviderDocument()
      {
        VanityUrl = "sp live atom $ &"
      };

      bool isValid = sp.IsVanityUrlValid();

      Assert.False(isValid);
    }

    [Fact]
    public void ReturnTrueWhenVanityUrlIsValid()
    {
      var sp = new ServiceProviderDocument()
      {
        VanityUrl = "spLiveatom22"
      };

      bool isValid = sp.IsVanityUrlValid();

      Assert.True(isValid);
    }
  }
}
