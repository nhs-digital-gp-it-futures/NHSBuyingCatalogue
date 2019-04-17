using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilitiesImplementedEvidenceDatastore : EvidenceDatastoreBase<CapabilitiesImplementedEvidence>, ICapabilitiesImplementedEvidenceDatastore
  {
    private readonly GifInt.ICapabilitiesImplementedEvidenceDatastore _crmDatastore;

    public CapabilitiesImplementedEvidenceDatastore(
      GifInt.ICapabilitiesImplementedEvidenceDatastore crmDatastore,
      ILogger<CrmDatastoreBase<CapabilitiesImplementedEvidence>> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    protected override IEnumerable<IEnumerable<CapabilitiesImplementedEvidence>> ByClaimInternal(string claimId)
    {
      var retval = new List<IEnumerable<CapabilitiesImplementedEvidence>>();
      var allVals = _crmDatastore
        .ByClaim(claimId);
      foreach (var vals in allVals)
      {
        retval.Add(vals.Select(val => Creator.FromCrm(val)));
      }
      return retval;
    }

    protected override CapabilitiesImplementedEvidence ByIdInternal(string id)
    {
      var val = _crmDatastore
        .ById(id);

      return Creator.FromCrm(val);
    }

    protected override CapabilitiesImplementedEvidence CreateInternal(CapabilitiesImplementedEvidence evidence)
    {
      var val = _crmDatastore
        .Create(Creator.FromApi(evidence));

      return Creator.FromCrm(val);
    }
  }
}
