using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Tests
{
  public sealed class DummyClaimsValidatorBase : ClaimsValidatorBase<ClaimsBase>
  {
    public DummyClaimsValidatorBase(
      IHttpContextAccessor context,
      IClaimsDatastore<ClaimsBase> claimDatastore,
      IContactsDatastore contactsDatastore,
      ISolutionsDatastore solutionsDatastore) :
      base(context, claimDatastore, contactsDatastore, solutionsDatastore)
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
