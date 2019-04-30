using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class ReviewsDatastoreBase<T> : CrmDatastoreBase<T>, IReviewsDatastore<ReviewsBase> where T : ReviewsBase
  {
    protected ReviewsDatastoreBase(
      ILogger<CrmDatastoreBase<T>> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
    }

    protected abstract IEnumerable<IEnumerable<T>> ByEvidenceInternal(string evidenceId);
    public IEnumerable<IEnumerable<T>> ByEvidence(string evidenceId)
    {
      return GetInternal(() =>
      {
        return ByEvidenceInternal(evidenceId);
      });
    }

    protected abstract T ByIdInternal(string id);
    public T ById(string id)
    {
      return GetInternal(() =>
      {
        return ByIdInternal(id);
      });
    }

    protected abstract T CreateInternal(T review);
    public T Create(T review)
    {
      return GetInternal(() =>
      {
        review.Id = UpdateId(review.Id);

        return CreateInternal(review);
      });
    }

    protected abstract void DeleteInternal(T review);
    public void Delete(T review)
    {
      GetInternal(() =>
      {
        DeleteInternal(review);

        return 0;
      });
    }

    IEnumerable<IEnumerable<ReviewsBase>> IReviewsDatastore<ReviewsBase>.ByEvidence(string evidenceId)
    {
      return ByEvidence(evidenceId);
    }

    ReviewsBase IReviewsDatastore<ReviewsBase>.ById(string id)
    {
      return ById(id);
    }

    ReviewsBase IReviewsDatastore<ReviewsBase>.Create(ReviewsBase review)
    {
      return Create((T)review);
    }

    void IReviewsDatastore<ReviewsBase>.Delete(ReviewsBase review)
    {
      Delete((T)review);
    }
  }
}
