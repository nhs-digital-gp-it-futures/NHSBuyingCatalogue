using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardsApplicableEvidenceDatastore : EvidenceDatastoreBase<StandardsApplicableEvidence>, IStandardsApplicableEvidenceDatastore
  {
    private readonly GifInt.IStandardsApplicableEvidenceDatastore _crmDatastore;

    public StandardsApplicableEvidenceDatastore(
      GifInt.IStandardsApplicableEvidenceDatastore crmDatastore,
      ILogger<CrmDatastoreBase<StandardsApplicableEvidence>> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    protected override IEnumerable<IEnumerable<StandardsApplicableEvidence>> ByClaimInternal(string claimId)
    {
      var retval = new List<IEnumerable<StandardsApplicableEvidence>>();
      var allVals = _crmDatastore
        .ByClaim(claimId);
      foreach (var vals in allVals)
      {
        retval.Add(vals.Select(val => Creator.FromCrm(val)));
      }
      return retval;
    }

    protected override StandardsApplicableEvidence ByIdInternal(string id)
    {
      var val = _crmDatastore
        .ById(id);

      return Creator.FromCrm(val);
    }

    protected override StandardsApplicableEvidence CreateInternal(StandardsApplicableEvidence evidence)
    {
      var val = _crmDatastore
        .Create(Creator.FromApi(evidence));

      return Creator.FromCrm(val);
    }
  }
}
