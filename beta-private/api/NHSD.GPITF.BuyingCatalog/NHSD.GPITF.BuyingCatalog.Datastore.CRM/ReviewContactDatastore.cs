using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class ReviewContactDatastore : DatastoreBase<ReviewContact>, IReviewContactDatastore
  {
    public ReviewContactDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<ReviewContactDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<ReviewContact> ByAssessmentMessage(string assMessId)
    {
      throw new NotImplementedException();
    }

    public ReviewContact Create(ReviewContact assMessCont)
    {
      throw new NotImplementedException();
    }

    public void Delete(ReviewContact assMessCont)
    {
      throw new NotImplementedException();
    }

    public void Update(ReviewContact assMessCont)
    {
      throw new NotImplementedException();
    }
  }
}
