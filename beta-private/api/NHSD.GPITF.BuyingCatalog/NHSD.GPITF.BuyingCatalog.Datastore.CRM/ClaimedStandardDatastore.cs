using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class ClaimedStandardDatastore : DatastoreBase<ClaimedStandard>, IClaimedStandardDatastore
  {
    public ClaimedStandardDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<ClaimedStandardDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<ClaimedStandard> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public ClaimedStandard Create(ClaimedStandard claimedstandard)
    {
      throw new NotImplementedException();
    }

    public void Delete(ClaimedStandard claimedstandard)
    {
      throw new NotImplementedException();
    }

    public void Update(ClaimedStandard claimedstandard)
    {
      throw new NotImplementedException();
    }
  }
}
