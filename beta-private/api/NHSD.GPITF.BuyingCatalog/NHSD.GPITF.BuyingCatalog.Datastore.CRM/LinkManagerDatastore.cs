using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class LinkManagerDatastore : DatastoreBase<object>, ILinkManagerDatastore
  {
    public LinkManagerDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<LinkManagerDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public void CapabilityFrameworkCreate(string frameworkId, string capabilityId)
    {
      throw new NotImplementedException();
    }

    public void CapabilityStandardCreate(string capabilityId, string standardId, bool isOptional)
    {
      throw new NotImplementedException();
    }

    public void FrameworkSolutionCreate(string frameworkId, string solutionId)
    {
      throw new NotImplementedException();
    }

    public void FrameworkStandardCreate(string frameworkId, string standardId)
    {
      throw new NotImplementedException();
    }
  }
}
