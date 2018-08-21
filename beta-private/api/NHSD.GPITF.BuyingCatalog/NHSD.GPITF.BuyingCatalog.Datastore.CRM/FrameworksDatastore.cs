using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

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

    public IQueryable<Frameworks> ByCapability(string capabilityId)
    {
      throw new NotImplementedException();
    }

    public Frameworks ById(string id)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Frameworks> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Frameworks> ByStandard(string standardId)
    {
      throw new NotImplementedException();
    }

    public Frameworks Create(Frameworks framework)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Frameworks> GetAll()
    {
      throw new NotImplementedException();
    }

    public void Update(Frameworks framework)
    {
      throw new NotImplementedException();
    }
  }
}
