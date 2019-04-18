using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class ShortTermCachedDatastore<T> : CachedDatastoreBase<T>
  {
    private readonly IShortTermCache _shortTermCache;

    public ShortTermCachedDatastore(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      IShortTermCache cache) :
      base(logger, policy, config, cache)
    {
      _shortTermCache = cache;
    }

    protected void ExpireValue(string path)
    {
      _shortTermCache.ExpireValue(path);
    }
  }
}
