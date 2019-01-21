using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using Polly;
using StackExchange.Redis;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class DatastoreCache : IDatastoreCache
  {
    private static TimeSpan Expiry = TimeSpan.FromDays(7);

    private readonly IConfiguration _config;
    private readonly ILogger<DatastoreCache> _logger;
    private readonly ISyncPolicy _policy;
    private readonly IDatabase _db;

    public DatastoreCache(
      IConfiguration config,
      ILogger<DatastoreCache> logger,
      ISyncPolicyFactory policy)
    {
      _config = config;
      _logger = logger;
      _policy = policy.Build(_logger);

      var cacheHost = Settings.CACHE_HOST(_config);
      var cfg = new ConfigurationOptions
      {
        EndPoints =
        {
          { cacheHost }
        },
        SyncTimeout = int.MaxValue
      };
      var redis = ConnectionMultiplexer.Connect(cfg);
      _db = redis.GetDatabase();
    }

    public void SafeAdd(string path, string jsonCachedResponse)
    {
      GetInternal(() =>
      {
        _db.StringSet(path, jsonCachedResponse, Expiry);
        return 0;
      });
    }

    public bool TryGetValue(string path, out string jsonCachedResponse)
    {
      var cacheVal = GetInternal(() =>
      {
        return _db.StringGet(path);
      });
      jsonCachedResponse = cacheVal;
      return cacheVal.HasValue;
    }

    private TOther GetInternal<TOther>(Func<TOther> get)
    {
      return _policy.Execute(get);
    }
  }
}
