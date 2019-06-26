using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
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
        retval.Add(vals.Select(val => Converter.FromCrm(val)));
      }

      return retval;
    }

    protected override CapabilitiesImplementedEvidence ByIdInternal(string id)
    {
      var val = _crmDatastore
        .ById(id);

      return Converter.FromCrm(val);
    }

    protected override CapabilitiesImplementedEvidence CreateInternal(CapabilitiesImplementedEvidence evidence)
    {
      var val = _crmDatastore
        .Create(Converter.FromApi(evidence));

      return Converter.FromCrm(val);
    }
  }
}
