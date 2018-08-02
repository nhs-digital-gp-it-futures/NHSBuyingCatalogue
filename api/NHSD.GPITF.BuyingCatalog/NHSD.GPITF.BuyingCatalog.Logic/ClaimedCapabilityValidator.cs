using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ClaimedCapabilityValidator : ValidatorBase<ClaimedCapability>, IClaimedCapabilityValidator
  {
    public ClaimedCapabilityValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
