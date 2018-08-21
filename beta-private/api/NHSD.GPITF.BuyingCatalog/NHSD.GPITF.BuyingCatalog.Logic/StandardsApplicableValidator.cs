using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableValidator : ValidatorBase<StandardsApplicable>, IStandardsApplicableValidator
  {
    public StandardsApplicableValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
