using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class ShortTermCache : CacheBase, IShortTermCache
  {
    public ShortTermCache(
      IConfiguration config,
      ILogger<ShortTermCache> logger,
      ISyncPolicyFactory policy) :
      base(config, logger, policy)
    {
    }

    protected override TimeSpan GetCacheExpiry(IConfiguration config)
    {
      return TimeSpan.FromSeconds(Settings.CRM_SHORT_TERM_CACHE_EXPIRY_SECS(config));
    }
  }
}
