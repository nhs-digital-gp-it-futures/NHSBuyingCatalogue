using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public sealed class StandardsApplicableEvidenceBlobStoreValidator : ClaimsEvidenceBlobStoreValidatorBase, ICapabilitiesImplementedEvidenceBlobStoreValidator
  {
    public StandardsApplicableEvidenceBlobStoreValidator(
      IHttpContextAccessor context,
      ISolutionsDatastore solutionsDatastore,
      IStandardsApplicableDatastore claimsDatastore) :
      base(context, solutionsDatastore, (IClaimsDatastore<ClaimsBase>)claimsDatastore)
    {
    }
  }
}
