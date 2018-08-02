using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ClaimedStandardValidator : ValidatorBase<ClaimedStandard>, IClaimedStandardValidator
  {
    public ClaimedStandardValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
