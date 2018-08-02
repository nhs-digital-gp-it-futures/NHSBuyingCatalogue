using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class ClaimedCapabilityDatastore : DatastoreBase<ClaimedCapability>, IClaimedCapabilityDatastore
  {
    public ClaimedCapabilityDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<ClaimedCapabilityDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<ClaimedCapability> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public ClaimedCapability Create(ClaimedCapability claimedcapability)
    {
      throw new NotImplementedException();
    }

    public void Delete(ClaimedCapability claimedcapability)
    {
      throw new NotImplementedException();
    }

    public void Update(ClaimedCapability claimedcapability)
    {
      throw new NotImplementedException();
    }
  }
}
