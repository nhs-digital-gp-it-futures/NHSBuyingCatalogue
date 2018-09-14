using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class LinkManagerValidator : ValidatorBase<object>, ILinkManagerValidator
  {
    public LinkManagerValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(ILinkManagerLogic.FrameworkSolutionCreate), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
