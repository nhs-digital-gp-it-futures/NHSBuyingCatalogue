using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilitiesImplementedDatastore : DatastoreBase<CapabilitiesImplemented>, ICapabilitiesImplementedDatastore
  {
    public CapabilitiesImplementedDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<CapabilitiesImplementedDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<CapabilitiesImplemented> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public CapabilitiesImplemented Create(CapabilitiesImplemented claimedcapability)
    {
      throw new NotImplementedException();
    }

    public void Delete(CapabilitiesImplemented claimedcapability)
    {
      throw new NotImplementedException();
    }

    public void Update(CapabilitiesImplemented claimedcapability)
    {
      throw new NotImplementedException();
    }
  }
}
