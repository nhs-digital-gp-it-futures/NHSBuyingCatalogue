using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardsApplicableDatastore : DatastoreBase<StandardsApplicable>, IStandardsApplicableDatastore
  {
    public StandardsApplicableDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<StandardsApplicableDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<StandardsApplicable> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public StandardsApplicable Create(StandardsApplicable claimedstandard)
    {
      throw new NotImplementedException();
    }

    public void Delete(StandardsApplicable claimedstandard)
    {
      throw new NotImplementedException();
    }

    public void Update(StandardsApplicable claimedstandard)
    {
      throw new NotImplementedException();
    }
  }
}
