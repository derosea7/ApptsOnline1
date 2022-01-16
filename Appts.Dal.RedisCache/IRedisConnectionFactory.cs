using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Appts.Dal.RedisCache
{
  public interface IRedisConnectionFactory
  {
    IConnectionMultiplexer Connection();
  }
}
