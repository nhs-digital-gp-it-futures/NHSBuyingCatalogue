using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class LongTermCachedDatastore<T> : CachedDatastoreBase<T>
  {
    public LongTermCachedDatastore(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      ILongTermCache cache) :
      base(logger, policy, config, cache)
    {
    }
  }
}
