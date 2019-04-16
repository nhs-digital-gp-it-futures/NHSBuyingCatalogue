using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilityStandardDatastore : CachedDatastore<CapabilityStandard>, ICapabilityStandardDatastore
  {
    private readonly GifInt.ICapabilityStandardDatastore _crmDatastore;

    public CapabilityStandardDatastore(
      GifInt.ICapabilityStandardDatastore crmDatastore,
      ILogger<CapabilityStandardDatastore> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      IDatastoreCache cache) :
      base(logger, policy, config, cache)
    {
      _crmDatastore = crmDatastore;
    }

    public IEnumerable<CapabilityStandard> GetAll()
    {
      return GetInternal(() =>
      {
        return GetAll($"/{nameof(CapabilityStandard)}");
      });
    }

    protected override IEnumerable<CapabilityStandard> GetAllInternal(string path)
    {
      var vals = _crmDatastore
        .GetAll()
        .Select(val => Creator.FromCrm(val));

      return vals;
    }

    protected override CapabilityStandard GetInternal(string path, string parameter)
    {
      throw new System.NotImplementedException();
    }
  }
}
