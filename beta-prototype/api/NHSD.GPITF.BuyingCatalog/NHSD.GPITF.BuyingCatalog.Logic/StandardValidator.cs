using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardValidator : ValidatorBase<Standard>, IStandardValidator
  {
    public StandardValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IStandardLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IStandardLogic.Update), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
