using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using Polly;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class CrmDatastoreBase<T>
  {
    protected readonly ILogger<CrmDatastoreBase<T>> _logger;
    private readonly ISyncPolicy _policy;

    public CrmDatastoreBase(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy)
    {
      _logger = logger;
      _policy = policy.Build(_logger);
    }

    protected TOther GetInternal<TOther>(Func<TOther> get)
    {
      return _policy.Execute(get);
    }
  }
}
