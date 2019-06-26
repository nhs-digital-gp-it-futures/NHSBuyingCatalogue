using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilitiesImplementedDatastore : ClaimsDatastoreBase<CapabilitiesImplemented>, ICapabilitiesImplementedDatastore
  {
    private readonly GifInt.ICapabilitiesImplementedDatastore _crmDatastore;

    public CapabilitiesImplementedDatastore(
      GifInt.ICapabilitiesImplementedDatastore crmDatastore,
      ILogger<CapabilitiesImplementedDatastore> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    protected override CapabilitiesImplemented ByIdInternal(string id)
    {
      var val = _crmDatastore
        .ById(id);

      return Converter.FromCrm(val);
    }

    protected override IEnumerable<CapabilitiesImplemented> BySolutionInternal(string solutionId)
    {
      var vals = _crmDatastore
        .BySolution(solutionId)
        .Select(val => Converter.FromCrm(val));

      return vals;
    }

    protected override CapabilitiesImplemented CreateInternal(CapabilitiesImplemented claim)
    {
      var val = _crmDatastore
        .Create(Converter.FromApi(claim));

      return Converter.FromCrm(val);
    }

    protected override void DeleteInternal(CapabilitiesImplemented claim)
    {
      _crmDatastore.Delete(Converter.FromApi(claim));
    }

    protected override void UpdateInternal(CapabilitiesImplemented claim)
    {
      _crmDatastore.Update(Converter.FromApi(claim));
    }
  }
}
