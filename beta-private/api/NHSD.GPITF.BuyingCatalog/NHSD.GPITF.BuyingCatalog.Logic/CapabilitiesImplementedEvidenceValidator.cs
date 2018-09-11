using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedEvidenceValidator : EvidenceValidatorBase<CapabilitiesImplementedEvidence>, ICapabilitiesImplementedEvidenceValidator
  {
    public CapabilitiesImplementedEvidenceValidator(
      ICapabilitiesImplementedEvidenceDatastore evidenceDatastore,
      ICapabilitiesImplementedDatastore claimDatastore,
      ISolutionsDatastore solutionDatastore,
      IHttpContextAccessor context) :
      base(evidenceDatastore, (IClaimsDatastore<ClaimsBase>)claimDatastore, solutionDatastore, context)
    {
    }

    protected override SolutionStatus SolutionReviewStatus => SolutionStatus.CapabilitiesAssessment;
  }
}
