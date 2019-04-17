using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Tests
{
  public sealed class DummyClaimsDatastoreBase : ClaimsDatastoreBase<ClaimsBase>
  {
    public DummyClaimsDatastoreBase(
      ILogger<CrmDatastoreBase<ClaimsBase>> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
    }

    protected override ClaimsBase ByIdInternal(string id)
    {
      throw new System.NotImplementedException();
    }

    protected override IEnumerable<ClaimsBase> BySolutionInternal(string solutionId)
    {
      throw new System.NotImplementedException();
    }

    protected override ClaimsBase CreateInternal(ClaimsBase claim)
    {
      throw new System.NotImplementedException();
    }

    protected override void DeleteInternal(ClaimsBase claim)
    {
      throw new System.NotImplementedException();
    }

    protected override void UpdateInternal(ClaimsBase claim)
    {
      throw new System.NotImplementedException();
    }
  }
}
