using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.Tests
{
  public sealed class DummyClaimsEvidenceBlobStoreValidatorBase : ClaimsEvidenceBlobStoreValidatorBase
  {
    public DummyClaimsEvidenceBlobStoreValidatorBase(
      IHttpContextAccessor context,
      ISolutionsDatastore solutionsDatastore,
      IClaimsDatastore<ClaimsBase> claimsDatastore) :
      base(context, solutionsDatastore, claimsDatastore)
    {
    }
  }
}
