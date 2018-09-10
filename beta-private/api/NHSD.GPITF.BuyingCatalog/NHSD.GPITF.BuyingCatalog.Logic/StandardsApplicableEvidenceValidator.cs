using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableEvidenceValidator : EvidenceValidatorBase<StandardsApplicableEvidence>, IStandardsApplicableEvidenceValidator
  {
    public StandardsApplicableEvidenceValidator(
      IStandardsApplicableDatastore claimDatastore,
      ISolutionsDatastore solutionDatastore,
      IHttpContextAccessor context) :
      base((IClaimsDatastore<ClaimsBase>)claimDatastore, solutionDatastore, context)
    {
    }

    protected override SolutionStatus SolutionReviewStatus => SolutionStatus.StandardsCompliance;
  }
}
