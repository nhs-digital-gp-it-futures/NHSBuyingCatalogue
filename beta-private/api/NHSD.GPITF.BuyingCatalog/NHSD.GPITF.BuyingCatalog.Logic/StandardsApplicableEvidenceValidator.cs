using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableEvidenceValidator : EvidenceValidatorBase<StandardsApplicableEvidence>, IStandardsApplicableEvidenceValidator
  {
    public StandardsApplicableEvidenceValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
