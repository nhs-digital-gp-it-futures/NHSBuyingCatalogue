using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class FrameworksValidator : ValidatorBase<Frameworks>, IFrameworksValidator
  {
    public FrameworksValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
