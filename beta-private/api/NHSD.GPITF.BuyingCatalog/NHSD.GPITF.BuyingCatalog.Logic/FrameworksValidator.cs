using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class FrameworksValidator : ValidatorBase<Frameworks>, IFrameworksValidator
  {
    public FrameworksValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IFrameworksLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IFrameworksLogic.Update), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
