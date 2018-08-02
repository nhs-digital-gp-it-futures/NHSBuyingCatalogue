using Microsoft.AspNetCore.Http;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class LinkManagerValidator : ValidatorBase<object>, ILinkManagerValidator
  {
    public LinkManagerValidator(IHttpContextAccessor context) :
      base(context)
    {
      MustBeAdmin();
    }
  }
}
