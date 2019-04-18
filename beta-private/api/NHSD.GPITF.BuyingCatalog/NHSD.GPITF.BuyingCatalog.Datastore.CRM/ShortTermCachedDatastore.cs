using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class ShortTermCachedDatastore<T> : CachedDatastoreBase<T>
  {
    public ShortTermCachedDatastore(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      IShortTermCache cache) :
      base(logger, policy, config, cache)
    {
    }

    protected IShortTermCache _shortTermCache => (IShortTermCache)_cache;
  }
}
