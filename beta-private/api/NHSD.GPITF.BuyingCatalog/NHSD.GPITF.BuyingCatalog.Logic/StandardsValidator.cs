using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsValidator : ValidatorBase<Standards>, IStandardsValidator
  {
    public StandardsValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IStandardsLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IStandardsLogic.Update), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
