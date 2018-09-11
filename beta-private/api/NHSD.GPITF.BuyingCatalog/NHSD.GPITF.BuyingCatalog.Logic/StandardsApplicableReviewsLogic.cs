using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableReviewsLogic : ReviewsLogicBase<StandardsApplicableReviews>, IStandardsApplicableReviewsLogic
  {
    public StandardsApplicableReviewsLogic(
      IStandardsApplicableReviewsDatastore datastore,
      IContactsDatastore contacts,
      IStandardsApplicableReviewsFilter filter,
      IHttpContextAccessor context) :
      base(datastore, contacts, filter, context)
    {
    }
  }
}
