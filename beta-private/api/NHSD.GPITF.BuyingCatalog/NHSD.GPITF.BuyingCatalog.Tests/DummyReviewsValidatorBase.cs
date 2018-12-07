using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Tests
{
  public sealed class DummyReviewsValidatorBase : ReviewsValidatorBase<DummyReviewsBase>
  {
    public DummyReviewsValidatorBase(
      IReviewsDatastore<ReviewsBase> reviewsDatastore,
      IEvidenceDatastore<EvidenceBase> evidenceDatastore,
      IClaimsDatastore<ClaimsBase> claimDatastore,
      ISolutionsDatastore solutionDatastore,
      IHttpContextAccessor context) :
      base(reviewsDatastore, evidenceDatastore, claimDatastore, solutionDatastore, context)
    {
    }

    protected override SolutionStatus SolutionReviewStatus => SolutionStatus.Failed;
  }
}
