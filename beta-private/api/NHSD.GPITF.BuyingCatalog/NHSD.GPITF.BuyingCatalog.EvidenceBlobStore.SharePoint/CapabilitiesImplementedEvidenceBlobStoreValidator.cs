using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public sealed class CapabilitiesImplementedEvidenceBlobStoreValidator : ClaimsEvidenceBlobStoreValidatorBase, ICapabilitiesImplementedEvidenceBlobStoreValidator
  {
    public CapabilitiesImplementedEvidenceBlobStoreValidator(
      IHttpContextAccessor context,
      ISolutionsDatastore solutionsDatastore,
      ICapabilitiesImplementedDatastore claimsDatastore) :
      base(context, solutionsDatastore, (IClaimsDatastore<ClaimsBase>)claimsDatastore)
    {
    }
  }
}
