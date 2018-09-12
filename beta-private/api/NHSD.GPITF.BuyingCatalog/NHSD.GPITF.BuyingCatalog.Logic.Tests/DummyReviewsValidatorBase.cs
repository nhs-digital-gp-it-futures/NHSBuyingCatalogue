using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  internal sealed class DummyReviewsValidatorBase : ReviewsValidatorBase<DummyReviewsBase>
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
