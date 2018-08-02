using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class SolutionDatastore : DatastoreBase<Solution>, ISolutionDatastore
  {
    public SolutionDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<SolutionDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Solution> ByFramework(string frameworkId)
    {
      throw new NotImplementedException();
    }

    public Solution ById(string id)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Solution> ByOrganisation(string organisationId)
    {
      throw new NotImplementedException();
    }

    public Solution Create(Solution solution)
    {
      throw new NotImplementedException();
    }

    public void Delete(Solution solution)
    {
      throw new NotImplementedException();
    }

    public void Update(Solution solution)
    {
      throw new NotImplementedException();
    }
  }
}
