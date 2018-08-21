using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class ReviewsDatastore : DatastoreBase<Reviews>, IReviewsDatastore
  {
    public ReviewsDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<ReviewsDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Reviews> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public Reviews Create(Reviews assMess)
    {
      throw new NotImplementedException();
    }

    public void Delete(Reviews assMess)
    {
      throw new NotImplementedException();
    }

    public void Update(Reviews assMess)
    {
      throw new NotImplementedException();
    }
  }
}
