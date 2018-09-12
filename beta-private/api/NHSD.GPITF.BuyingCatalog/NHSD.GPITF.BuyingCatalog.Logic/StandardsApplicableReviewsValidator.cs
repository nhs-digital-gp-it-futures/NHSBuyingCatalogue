using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableReviewsValidator : ReviewsValidatorBase<StandardsApplicableReviews>, IStandardsApplicableReviewsValidator
  {
    public StandardsApplicableReviewsValidator(
      IStandardsApplicableReviewsDatastore datastore,
      IStandardsApplicableEvidenceDatastore evidenceDatastore,
      IStandardsApplicableDatastore claimDatastore,
      ISolutionsDatastore solutionDatastore,
      IHttpContextAccessor context) :
      base((IReviewsDatastore<ReviewsBase>)datastore, (IEvidenceDatastore<EvidenceBase>)evidenceDatastore, (IClaimsDatastore<ClaimsBase>)claimDatastore, solutionDatastore, context)
    {
    }

    protected override SolutionStatus SolutionReviewStatus => SolutionStatus.StandardsCompliance;
  }
}
