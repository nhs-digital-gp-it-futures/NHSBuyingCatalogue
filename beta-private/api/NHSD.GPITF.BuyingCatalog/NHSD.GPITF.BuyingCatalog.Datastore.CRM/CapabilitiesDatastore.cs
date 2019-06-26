using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilitiesDatastore : LongTermCachedDatastore<Capabilities>, ICapabilitiesDatastore
  {
    private readonly GifInt.ICapabilityDatastore _crmDatastore;

    public CapabilitiesDatastore(
      GifInt.ICapabilityDatastore crmDatastore,
      ILogger<CapabilitiesDatastore> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      ILongTermCache cache) :
      base(logger, policy, config, cache)
    {
      _crmDatastore = crmDatastore;
    }

    public IEnumerable<Capabilities> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByFramework(frameworkId)
          .Select(val => Converter.FromCrm(val));

        return vals;
      });
    }

    public Capabilities ById(string id)
    {
      return GetInternal(() =>
      {
        return GetAll().SingleOrDefault(x => x.Id == id);
      });
    }

    public IEnumerable<Capabilities> ByIds(IEnumerable<string> ids)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByIds(ids)
          .Select(val => Converter.FromCrm(val));

        return vals;
      });
    }

    public IEnumerable<Capabilities> ByStandard(string standardId, bool isOptional)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByStandard(standardId, isOptional)
          .Select(val => Converter.FromCrm(val));

        return vals;
      });
    }

    public IEnumerable<Capabilities> GetAll()
    {
      return GetInternal(() =>
      {
        return GetAllFromCache($"/{nameof(Capabilities)}");
      });
    }

    protected override Capabilities GetFromSource(string path, string parameter)
    {
      throw new NotImplementedException();
    }

    protected override IEnumerable<Capabilities> GetAllFromSource(string path, string parameter = null)
    {
      var vals = _crmDatastore
        .GetAll()
        .Select(val => Converter.FromCrm(val));

      return vals;
    }
  }
}
