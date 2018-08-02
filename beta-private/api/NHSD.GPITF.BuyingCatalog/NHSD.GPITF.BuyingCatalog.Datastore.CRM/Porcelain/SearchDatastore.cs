using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain
{
  public sealed class SearchDatastore : DatastoreBase<SolutionEx>, ISearchDatastore
  {
    public SearchDatastore(
      IRestClientFactory crmConnectionFactory, 
      ILogger<SearchDatastore> logger, 
      ISyncPolicyFactory policy) : 
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<SolutionEx> SolutionExByKeyword(string keyword)
    {
      throw new NotImplementedException();
    }
  }
}
