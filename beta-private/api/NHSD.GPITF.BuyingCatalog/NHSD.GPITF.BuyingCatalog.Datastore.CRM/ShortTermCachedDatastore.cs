using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class ShortTermCachedDatastore<T> : CachedDatastoreBase<T>, IOtherCache
  {
    protected readonly IServiceProvider _serviceProvider;
    private readonly IShortTermCache _shortTermCache;

    protected ShortTermCachedDatastore(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      IShortTermCache cache,
      IServiceProvider serviceProvider) :
      base(logger, policy, config, cache)
    {
      _shortTermCache = cache;
      _serviceProvider = serviceProvider;
    }

    public abstract void ExpireOtherValue(object item);

    protected void ExpireValue(string path)
    {
      _shortTermCache.ExpireValue(path);
    }
  }
}
