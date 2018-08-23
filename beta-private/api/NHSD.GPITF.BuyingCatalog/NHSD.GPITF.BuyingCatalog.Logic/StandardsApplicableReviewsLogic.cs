using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableReviewsLogic : LogicBase, IStandardsApplicableReviewsLogic
  {
    private readonly IStandardsApplicableReviewsDatastore _datastore;
    private readonly IContactsDatastore _contacts;

    public StandardsApplicableReviewsLogic(
      IStandardsApplicableReviewsDatastore datastore,
      IContactsDatastore contacts,
      IHttpContextAccessor context) :
      base(context)
    {
      _datastore = datastore;
      _contacts = contacts;
    }

    public IQueryable<StandardsApplicableReviews> ByEvidence(string evidenceId)
    {
      return _datastore.ByEvidence(evidenceId);
    }

    public StandardsApplicableReviews Create(StandardsApplicableReviews review)
    {
      var email = Context.Email();
      review.CreatedById = _contacts.ByEmail(email).Id;
      return _datastore.Create(review);
    }
  }
}
