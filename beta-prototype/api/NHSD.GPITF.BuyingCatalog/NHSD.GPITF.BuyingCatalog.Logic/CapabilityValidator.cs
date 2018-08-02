using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilityValidator : ValidatorBase<Capability>, ICapabilityValidator
  {
    public CapabilityValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(ICapabilityLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(ICapabilityLogic.Update), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
