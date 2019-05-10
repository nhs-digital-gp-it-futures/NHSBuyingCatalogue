using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class LongTermCache : CacheBase, ILongTermCache
  {
    public LongTermCache(
      IConfiguration config,
      ILogger<LongTermCache> logger,
      ISyncPolicyFactory policy) :
      base(config, logger, policy)
    {
    }

    protected override TimeSpan GetCacheExpiry(IConfiguration config)
    {
      return TimeSpan.FromMinutes(Settings.CRM_CACHE_EXPIRY_MINS(config));
    }
  }
}
