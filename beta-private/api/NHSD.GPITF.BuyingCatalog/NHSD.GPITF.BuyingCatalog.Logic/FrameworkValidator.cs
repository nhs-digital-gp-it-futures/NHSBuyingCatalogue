using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class FrameworkValidator : ValidatorBase<Framework>, IFrameworkValidator
  {
    public FrameworkValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IFrameworkLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IFrameworkLogic.Update), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
