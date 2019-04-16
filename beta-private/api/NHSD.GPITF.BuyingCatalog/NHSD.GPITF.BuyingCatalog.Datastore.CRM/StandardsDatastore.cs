using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardsDatastore : CachedDatastore<Standards>, IStandardsDatastore
  {
    private readonly GifInt.IStandardsDatastore _crmDatastore;

    public StandardsDatastore(
      GifInt.IStandardsDatastore crmDatastore,
      ILogger<StandardsDatastore> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      IDatastoreCache cache) :
      base(logger, policy, config, cache)
    {
      _crmDatastore = crmDatastore;
    }

    public IEnumerable<Standards> ByCapability(string capabilityId, bool isOptional)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByCapability(capabilityId, isOptional)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }

    public IEnumerable<Standards> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByFramework(frameworkId)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }

    public Standards ById(string id)
    {
      return GetInternal(() =>
      {
        return GetAll().SingleOrDefault(x => x.Id == id);
      });
    }

    public IEnumerable<Standards> ByIds(IEnumerable<string> ids)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByIds(ids)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }

    public IEnumerable<Standards> GetAll()
    {
      return GetInternal(() =>
      {
        return GetAll($"/{nameof(Standards)}");
      });
    }

    protected override Standards GetInternal(string path)
    {
      throw new System.NotImplementedException();
    }

    protected override IEnumerable<Standards> GetAllInternal(string path)
    {
      var vals = _crmDatastore
        .GetAll()
        .Select(val => Creator.FromCrm(val));

      return vals;
    }
  }
}
