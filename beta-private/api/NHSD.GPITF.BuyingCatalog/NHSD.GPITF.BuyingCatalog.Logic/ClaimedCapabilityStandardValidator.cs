using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ClaimedCapabilityStandardValidator : ValidatorBase<ClaimedCapabilityStandard>, IClaimedCapabilityStandardValidator
  {
    public ClaimedCapabilityStandardValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
