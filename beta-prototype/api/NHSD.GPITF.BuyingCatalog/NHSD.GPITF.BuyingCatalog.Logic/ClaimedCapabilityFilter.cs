using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ClaimedCapabilityFilter : FilterBase<ClaimedCapability>, IClaimedCapabilityFilter
  {
    public ClaimedCapabilityFilter(IHttpContextAccessor context) :
      base(context)
    {
    }

    protected override ClaimedCapability Filter(ClaimedCapability input)
    {
      return input;
    }
  }
}
