using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class StandardsApplicableReviewsDatastore : DatastoreBase<StandardsApplicableReviews>, IStandardsApplicableReviewsDatastore
  {
    public StandardsApplicableReviewsDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<StandardsApplicableReviewsDatastore> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<StandardsApplicableReviews> ByEvidence(string evidenceId)
    {
      return GetInternal(() =>
      {
        var retval = _dbConnection.Value.GetAll<StandardsApplicableReviews>().Where(sar => sar.StandardsApplicableEvidenceId == evidenceId);
        return retval.AsQueryable();
      });
    }

    public StandardsApplicableReviews Create(StandardsApplicableReviews review)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          review.Id = Guid.NewGuid().ToString();
          review.CreatedOn = DateTime.UtcNow;
          _dbConnection.Value.Insert(review, trans);
          trans.Commit();

          return review;
        }
      });
    }
  }
}
