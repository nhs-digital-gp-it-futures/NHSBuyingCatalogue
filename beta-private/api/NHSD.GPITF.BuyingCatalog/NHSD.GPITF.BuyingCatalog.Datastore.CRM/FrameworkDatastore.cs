using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class FrameworkDatastore : DatastoreBase<Framework>, IFrameworkDatastore
  {
    public FrameworkDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<FrameworkDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Framework> ByCapability(string capabilityId)
    {
      throw new NotImplementedException();
    }

    public Framework ById(string id)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Framework> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Framework> ByStandard(string standardId)
    {
      throw new NotImplementedException();
    }

    public Framework Create(Framework framework)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Framework> GetAll()
    {
      throw new NotImplementedException();
    }

    public void Update(Framework framework)
    {
      throw new NotImplementedException();
    }
  }
}
