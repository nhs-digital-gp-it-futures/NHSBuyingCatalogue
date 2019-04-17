using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Tests
{
  public sealed class DummyEvidenceDatastoreBase : EvidenceDatastoreBase<EvidenceBase>
  {
    public DummyEvidenceDatastoreBase(
      ILogger<CrmDatastoreBase<EvidenceBase>> logger, 
      ISyncPolicyFactory policy) : 
      base(logger, policy)
    {
    }

    protected override IEnumerable<IEnumerable<EvidenceBase>> ByClaimInternal(string claimId)
    {
      throw new System.NotImplementedException();
    }

    protected override EvidenceBase ByIdInternal(string id)
    {
      throw new System.NotImplementedException();
    }

    protected override EvidenceBase CreateInternal(EvidenceBase evidence)
    {
      throw new System.NotImplementedException();
    }
  }
}
