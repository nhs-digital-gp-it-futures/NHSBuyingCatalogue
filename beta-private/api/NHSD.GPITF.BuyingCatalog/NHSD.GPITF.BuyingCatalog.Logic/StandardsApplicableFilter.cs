using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableFilter : FilterBase<StandardsApplicable>, IStandardsApplicableFilter
  {
    public StandardsApplicableFilter(IHttpContextAccessor context) :
      base(context)
    {
    }

    protected override StandardsApplicable Filter(StandardsApplicable input)
    {
      return input;
    }
  }
}
