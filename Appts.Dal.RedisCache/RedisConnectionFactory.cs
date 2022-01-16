using StackExchange.Redis;
using System;

namespace Appts.Dal.RedisCache
{
  public class RedisConnectionFactory : IRedisConnectionFactory
  {
    /// <summary>
    ///     The _connection.
    /// </summary>
    private readonly Lazy<ConnectionMultiplexer> _connection;

    public RedisConnectionFactory(string password)
    {
      this._connection = new Lazy<ConnectionMultiplexer>(
        () => ConnectionMultiplexer.Connect(
          $"appts.redis.cache.windows.net,abortConnect=false,ssl=true,password={password}"));
    }

    public IConnectionMultiplexer Connection()
    {
      return this._connection.Value;
    }
  }
}
