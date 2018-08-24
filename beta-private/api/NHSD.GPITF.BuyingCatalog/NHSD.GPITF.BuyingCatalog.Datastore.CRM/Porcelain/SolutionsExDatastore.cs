using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain
{
  public sealed class SolutionsExDatastore : DatastoreBase<SolutionEx>, ISolutionsExDatastore
  {
    public SolutionsExDatastore(
      IRestClientFactory crmConnectionFactory, 
      ILogger<SolutionsExDatastore> logger, 
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public SolutionEx BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public void Update(SolutionEx solnEx)
    {
      throw new NotImplementedException();
    }
  }
}
