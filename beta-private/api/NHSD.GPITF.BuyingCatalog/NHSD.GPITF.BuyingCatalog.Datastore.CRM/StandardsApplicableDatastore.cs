using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardsApplicableDatastore : ClaimsDatastoreBase<StandardsApplicable>, IStandardsApplicableDatastore
  {
    private readonly GifInt.IStandardsApplicableDatastore _crmDatastore;

    public StandardsApplicableDatastore(
      GifInt.IStandardsApplicableDatastore crmDatastore,
      ILogger<StandardsApplicableDatastore> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    protected override StandardsApplicable ByIdInternal(string id)
    {
      var val = _crmDatastore
        .ById(id);

      return Converter.FromCrm(val);
    }

    protected override IEnumerable<StandardsApplicable> BySolutionInternal(string solutionId)
    {
      var vals = _crmDatastore
        .BySolution(solutionId)
        .Select(val => Converter.FromCrm(val));

      return vals;
    }

    protected override StandardsApplicable CreateInternal(StandardsApplicable claim)
    {
      var val = _crmDatastore
        .Create(Converter.FromApi(claim));

      return Converter.FromCrm(val);
    }

    protected override void DeleteInternal(StandardsApplicable claim)
    {
      _crmDatastore.Delete(Converter.FromApi(claim));
    }

    protected override void UpdateInternal(StandardsApplicable claim)
    {
      _crmDatastore.Update(Converter.FromApi(claim));
    }
  }
}
