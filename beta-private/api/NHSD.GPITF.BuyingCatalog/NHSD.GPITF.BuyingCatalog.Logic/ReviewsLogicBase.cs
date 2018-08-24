using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ReviewsLogicBase<T> : LogicBase where T : ReviewsBase
  {
    private readonly IReviewsDatastore<T> _datastore;
    private readonly IContactsDatastore _contacts;

    public ReviewsLogicBase(
      IReviewsDatastore<T> datastore,
      IContactsDatastore contacts,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
      _contacts = contacts;
    }

    public IQueryable<T> ByEvidence(string evidenceId)
    {
      return _datastore.ByEvidence(evidenceId);
    }

    public T Create(T review)
    {
      var email = Context.Email();
      review.CreatedById = _contacts.ByEmail(email).Id;
      return _datastore.Create(review);
    }
  }
}
