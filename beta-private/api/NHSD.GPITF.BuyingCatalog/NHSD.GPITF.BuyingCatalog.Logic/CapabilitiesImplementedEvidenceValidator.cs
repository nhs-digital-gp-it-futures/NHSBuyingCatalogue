using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedEvidenceValidator : EvidenceValidatorBase<CapabilitiesImplementedEvidence>, ICapabilitiesImplementedEvidenceValidator
  {
    public CapabilitiesImplementedEvidenceValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
