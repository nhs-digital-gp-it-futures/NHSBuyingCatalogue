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

      return Creator.FromCrm(val);
    }

    protected override IEnumerable<StandardsApplicable> BySolutionInternal(string solutionId)
    {
      var vals = _crmDatastore
        .BySolution(solutionId)
        .Select(val => Creator.FromCrm(val));

      return vals;
    }

    protected override StandardsApplicable CreateInternal(StandardsApplicable claim)
    {
      var val = _crmDatastore
        .Create(Creator.FromApi(claim));

      return Creator.FromCrm(val);
    }

    protected override void DeleteInternal(StandardsApplicable claim)
    {
      _crmDatastore.Delete(Creator.FromApi(claim));
    }

    protected override void UpdateInternal(StandardsApplicable claim)
    {
      _crmDatastore.Update(Creator.FromApi(claim));
    }
  }
}
