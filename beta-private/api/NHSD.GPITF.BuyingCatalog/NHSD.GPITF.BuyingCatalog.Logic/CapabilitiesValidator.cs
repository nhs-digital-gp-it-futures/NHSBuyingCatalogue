using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesValidator : ValidatorBase<Capabilities>, ICapabilitiesValidator
  {
    public CapabilitiesValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(ICapabilitiesLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(ICapabilitiesLogic.Update), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
