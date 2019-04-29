using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using Polly;
using StackExchange.Redis;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class CacheBase : ICache
  {
    private readonly TimeSpan _expiry;
    private readonly ISyncPolicy _policy;
    private readonly ConnectionMultiplexer _redis;

    protected CacheBase(
      IConfiguration config,
      ILogger<CacheBase> logger,
      ISyncPolicyFactory policy)
    {
      _policy = policy.Build(logger);

      _expiry = GetCacheExpiry(config);

      var cacheHost = Settings.CACHE_HOST(config);
      var cfg = new ConfigurationOptions
      {
        EndPoints =
        {
          { cacheHost }
        },
        SyncTimeout = int.MaxValue
      };
      _redis = ConnectionMultiplexer.Connect(cfg);
    }

    protected abstract TimeSpan GetCacheExpiry(IConfiguration config);

    public void SafeAdd(string path, string jsonCachedResponse)
    {
      GetInternal(() =>
      {
        _redis.GetDatabase().StringSet(path, jsonCachedResponse, _expiry);
        return 0;
      });
    }

    public bool TryGetValue(string path, out string jsonCachedResponse)
    {
      var cacheVal = GetInternal(() =>
      {
        return _redis.GetDatabase().StringGet(path);
      });
      jsonCachedResponse = cacheVal;
      return cacheVal.HasValue;
    }

    public void ExpireValue(string path)
    {
      GetInternal(() =>
      {
        _redis.GetDatabase().KeyDelete(path);
        return 0;
      });
    }

    private TOther GetInternal<TOther>(Func<TOther> get)
    {
      return _policy.Execute(get);
    }
  }
}
