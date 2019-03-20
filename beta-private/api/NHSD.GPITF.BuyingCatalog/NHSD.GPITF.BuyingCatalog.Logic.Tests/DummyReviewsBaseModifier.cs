using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Tests;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  public sealed class DummyReviewsBaseModifier : ReviewsBaseModifier<DummyReviewsBase>
  {
    public DummyReviewsBaseModifier(
      IHttpContextAccessor context,
      IContactsDatastore contacts) :
      base(context, contacts)
    {
    }
  }
}
