using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  public sealed class DummyClaimsLogicBase : ClaimsLogicBase<ClaimsBase>
  {
    public DummyClaimsLogicBase(
      IClaimsDatastore<ClaimsBase> datastore,
      IClaimsValidator<ClaimsBase> validator,
      IClaimsFilter<ClaimsBase> filter,
      IHttpContextAccessor context) :
      base(datastore, validator, filter, context)
    {
    }
  }
}
