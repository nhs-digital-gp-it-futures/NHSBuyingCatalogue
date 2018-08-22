using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsValidator : ValidatorBase<Standards>, IStandardsValidator
  {
    public StandardsValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
