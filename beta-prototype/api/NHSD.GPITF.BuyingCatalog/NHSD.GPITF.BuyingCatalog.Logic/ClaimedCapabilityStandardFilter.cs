using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ClaimedCapabilityStandardFilter : FilterBase<ClaimedCapabilityStandard>, IClaimedCapabilityStandardFilter
  {
    public ClaimedCapabilityStandardFilter(IHttpContextAccessor context) :
      base(context)
    {
    }

    protected override ClaimedCapabilityStandard Filter(ClaimedCapabilityStandard input)
    {
      return input;
    }
  }
}
