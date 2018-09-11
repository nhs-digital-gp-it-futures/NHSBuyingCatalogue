using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  internal sealed class DummyClaimsValidatorBase : ClaimsValidatorBase<ClaimsBase>
  {
    public DummyClaimsValidatorBase(
      IHttpContextAccessor context,
      IClaimsDatastore<ClaimsBase> claimDatastore,
      ISolutionsDatastore solutionsDatastore) :
      base(context, claimDatastore, solutionsDatastore)
    {
    }

    public override void MustBePending()
    {
    }

    public override void MustBeValidStatusTransition()
    {
    }
  }
}
