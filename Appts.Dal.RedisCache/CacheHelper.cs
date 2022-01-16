using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Dal.RedisCache
{
  public class CacheHelper
  {
    public static string GetGoogleCalendarTokenKey(string userId, string tokenType)
    {
      return $"user:{userId.ToLower()}:google:cal:{tokenType.ToLower()}:token";
    }
  }
}
