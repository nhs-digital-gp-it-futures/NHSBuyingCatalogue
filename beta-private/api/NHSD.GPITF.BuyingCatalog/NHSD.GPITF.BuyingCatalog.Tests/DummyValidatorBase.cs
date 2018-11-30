using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Logic;

namespace NHSD.GPITF.BuyingCatalog.Tests
{
  public sealed class DummyValidatorBase : ValidatorBase<object>
  {
    public DummyValidatorBase(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
