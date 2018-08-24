using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public abstract class ReviewsDatastoreBase<T> : DatastoreBase<T> where T : ReviewsBase
  {
    public ReviewsDatastoreBase(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<ReviewsDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<T> ByEvidence(string evidenceId)
    {
      return GetInternal(() =>
      {
        var retval = _dbConnection.Value.GetAll<T>().Where(rb => rb.EvidenceId == evidenceId);
        return retval.AsQueryable();
      });
    }

    public T Create(T review)
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
