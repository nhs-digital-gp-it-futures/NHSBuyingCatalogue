using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ClaimedStandardFilter : FilterBase<ClaimedStandard>, IClaimedStandardFilter
  {
    public ClaimedStandardFilter(IHttpContextAccessor context) :
      base(context)
    {
    }

    protected override ClaimedStandard Filter(ClaimedStandard input)
    {
      return input;
    }
  }
}
