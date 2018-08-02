using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class TechnicalContactDatastore : DatastoreBase<TechnicalContact>, ITechnicalContactDatastore
  {
    public TechnicalContactDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<TechnicalContactDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<TechnicalContact> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public TechnicalContact Create(TechnicalContact techCont)
    {
      throw new NotImplementedException();
    }

    public void Delete(TechnicalContact techCont)
    {
      throw new NotImplementedException();
    }

    public void Update(TechnicalContact techCont)
    {
      throw new NotImplementedException();
    }
  }
}
