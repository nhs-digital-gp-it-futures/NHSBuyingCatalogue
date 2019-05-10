using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class FrameworksDatastore : CrmDatastoreBase<Frameworks>, IFrameworksDatastore
  {
    private readonly GifInt.IFrameworksDatastore _crmDatastore;

    public FrameworksDatastore(
      GifInt.IFrameworksDatastore crmDatastore,
      ILogger<FrameworksDatastore> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    public IEnumerable<Frameworks> ByCapability(string capabilityId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByCapability(capabilityId)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }

    public Frameworks ById(string id)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ById(id);

        return Creator.FromCrm(vals);
      });
    }

    public IEnumerable<Frameworks> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .BySolution(solutionId)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }

    public IEnumerable<Frameworks> ByStandard(string standardId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByStandard(standardId)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }

    public IEnumerable<Frameworks> GetAll()
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .GetAll()
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }
  }
}
