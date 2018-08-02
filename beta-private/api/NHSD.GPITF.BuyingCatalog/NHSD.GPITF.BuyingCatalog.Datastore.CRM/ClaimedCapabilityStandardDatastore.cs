using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class ClaimedCapabilityStandardDatastore : DatastoreBase<ClaimedCapabilityStandard>, IClaimedCapabilityStandardDatastore
  {
    public ClaimedCapabilityStandardDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<ClaimedCapabilityStandardDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<ClaimedCapabilityStandard> ByClaimedCapability(string claimedCapabilityId)
    {
      throw new NotImplementedException();
    }

    public IQueryable<ClaimedCapabilityStandard> ByStandard(string standardId)
    {
      throw new NotImplementedException();
    }

    public ClaimedCapabilityStandard Create(ClaimedCapabilityStandard claimedCapStd)
    {
      throw new NotImplementedException();
    }

    public void Delete(ClaimedCapabilityStandard claimedCapStd)
    {
      throw new NotImplementedException();
    }

    public void Update(ClaimedCapabilityStandard claimedCapStd)
    {
      throw new NotImplementedException();
    }
  }
}
