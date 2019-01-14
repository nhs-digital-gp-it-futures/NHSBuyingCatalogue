using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class CachedDatastore<T> : DatastoreBase<T>
  {
    private readonly bool _logCRM;
    protected readonly IDatastoreCache _cache;

    public CachedDatastore(
      IRestClientFactory crmFactory, 
      ILogger<DatastoreBase<T>> logger, 
      ISyncPolicyFactory policy, 
      IConfiguration config,
      IDatastoreCache cache) : 
      base(crmFactory, logger, policy, config)
    {
      _logCRM = Settings.LOG_CRM(config);
      _cache = cache;
    }

    protected void LogInformation(string msg)
    {
      if (_logCRM)
      {
        _logger.LogInformation(msg);
      }
    }
  }
}
