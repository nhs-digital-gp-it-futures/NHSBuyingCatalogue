using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  internal sealed class DummyClaimsValidatorBase : ClaimsValidatorBase<ClaimsBase>
  {
    public DummyClaimsValidatorBase(
      IHttpContextAccessor context,
      IClaimsDatastore<ClaimsBase> claimDatastore) :
      base(context, claimDatastore)
    {
    }

    protected override void RuleForDelete()
    {
    }
  }
}
