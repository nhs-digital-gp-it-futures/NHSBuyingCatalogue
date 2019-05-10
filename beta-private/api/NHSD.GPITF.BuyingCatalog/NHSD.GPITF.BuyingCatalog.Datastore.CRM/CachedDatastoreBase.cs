using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class CachedDatastoreBase<T> : CrmDatastoreBase<T>
  {
    private readonly bool _logCRM;
    private readonly ICache _cache;

    protected CachedDatastoreBase(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      ICache cache) :
      base(logger, policy)
    {
      _logCRM = Settings.LOG_CRM(config);
      _cache = cache;
    }

    protected abstract T GetFromSource(string path, string parameter);
    protected T GetFromCache(string path, string parameter)
    {
      LogInformation($"[{path}]");
      if (_cache.TryGetValue(path, out string jsonCachedResponse))
      {
        LogInformation($"cache[{path}] --> [{jsonCachedResponse}]");
        return JsonConvert.DeserializeObject<T>(jsonCachedResponse);
      }

      var retval = GetFromSource(path, parameter);

      _cache.SafeAdd(path, JsonConvert.SerializeObject(retval));

      return retval;
    }

    protected abstract IEnumerable<T> GetAllFromSource(string path, string parameter = null);
    protected IEnumerable<T> GetAllFromCache(string path, string parameter = null)
    {
      LogInformation($"[{path}]");
      if (_cache.TryGetValue(path, out string jsonCachedResponse))
      {
        LogInformation($"cache[{path}] --> [{jsonCachedResponse}]");
        return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonCachedResponse);
      }

      var retval = GetAllFromSource(path, parameter);

      _cache.SafeAdd(path, JsonConvert.SerializeObject(retval));

      return retval;
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
