using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class CachedDatastore<T> : CrmDatastoreBase<T>
  {
    private readonly bool _logCRM;
    protected readonly IDatastoreCache _cache;

    public CachedDatastore(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      IDatastoreCache cache) :
      base(logger, policy)
    {
      _logCRM = Settings.LOG_CRM(config);
      _cache = cache;
    }

    protected abstract T GetInternal(string path, string parameter);
    protected T Get(string path, string parameter)
    {
      LogInformation($"[{path}]");
      if (_cache.TryGetValue(path, out string jsonCachedResponse))
      {
        LogInformation($"cache[{path}] --> [{jsonCachedResponse}]");
        return JsonConvert.DeserializeObject<T>(jsonCachedResponse);
      }

      var retval = GetInternal(path, parameter);

      _cache.SafeAdd(path, JsonConvert.SerializeObject(retval));

      return retval;
  }

    protected abstract IEnumerable<T> GetAllInternal(string path);
    protected IEnumerable<T> GetAll(string path)
    {
      LogInformation($"[{path}]");
      if (_cache.TryGetValue(path, out string jsonCachedResponse))
      {
        LogInformation($"cache[{path}] --> [{jsonCachedResponse}]");
        return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonCachedResponse);
      }

      var retval = GetAllInternal(path);

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
