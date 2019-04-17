using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Tests
{
  public sealed class DummyReviewsDatastoreBase : ReviewsDatastoreBase<ReviewsBase>
  {
    public DummyReviewsDatastoreBase(
      ILogger<ReviewsDatastoreBase<ReviewsBase>> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
    }

    protected override IEnumerable<IEnumerable<ReviewsBase>> ByEvidenceInternal(string evidenceId)
    {
      throw new System.NotImplementedException();
    }

    protected override ReviewsBase ByIdInternal(string id)
    {
      throw new System.NotImplementedException();
    }

    protected override ReviewsBase CreateInternal(ReviewsBase review)
    {
      throw new System.NotImplementedException();
    }

    protected override void DeleteInternal(ReviewsBase review)
    {
      throw new System.NotImplementedException();
    }
  }
}
