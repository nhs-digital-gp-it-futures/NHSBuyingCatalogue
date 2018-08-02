using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain
{
  public sealed class SolutionExDatastore : DatastoreBase<SolutionEx>, ISolutionExDatastore
  {
    public SolutionExDatastore(
      IRestClientFactory crmConnectionFactory, 
      ILogger<SolutionExDatastore> logger, 
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
