using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedReviewsValidator : ReviewsValidatorBase<CapabilitiesImplementedReviews>, ICapabilitiesImplementedReviewsValidator
  {
    public CapabilitiesImplementedReviewsValidator(
      ICapabilitiesImplementedReviewsDatastore datastore,
      ICapabilitiesImplementedEvidenceDatastore evidenceDatastore,
      ICapabilitiesImplementedDatastore claimDatastore,
      ISolutionsDatastore solutionDatastore,
      IHttpContextAccessor context) :
      base((IReviewsDatastore<ReviewsBase>)datastore, (IEvidenceDatastore<EvidenceBase>)evidenceDatastore, (IClaimsDatastore<ClaimsBase>)claimDatastore, solutionDatastore, context)
    {
    }

    protected override SolutionStatus SolutionReviewStatus => SolutionStatus.CapabilitiesAssessment;
  }
}
