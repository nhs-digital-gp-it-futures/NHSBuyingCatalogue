using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedFilter : FilterBase<CapabilitiesImplemented>, ICapabilitiesImplementedFilter
  {
    public CapabilitiesImplementedFilter(IHttpContextAccessor context) :
      base(context)
    {
    }

    protected override CapabilitiesImplemented Filter(CapabilitiesImplemented input)
    {
      return input;
    }
  }
}
