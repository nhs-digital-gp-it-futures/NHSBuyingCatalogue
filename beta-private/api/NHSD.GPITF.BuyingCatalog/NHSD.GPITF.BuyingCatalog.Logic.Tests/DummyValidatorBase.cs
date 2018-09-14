using Microsoft.AspNetCore.Http;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  internal sealed class DummyValidatorBase : ValidatorBase<object>
  {
    public DummyValidatorBase(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
