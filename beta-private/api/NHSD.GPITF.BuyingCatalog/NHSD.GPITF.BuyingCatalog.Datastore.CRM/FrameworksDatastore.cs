using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class FrameworksDatastore : DatastoreBase<Frameworks>, IFrameworksDatastore
  {
    public FrameworksDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<FrameworksDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IEnumerable<Frameworks> ByCapability(string capabilityId)
    {
      throw new NotImplementedException();
    }

    public Frameworks ById(string id)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<Frameworks> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<Frameworks> ByStandard(string standardId)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<Frameworks> GetAll()
    {
      throw new NotImplementedException();
    }
  }
}
