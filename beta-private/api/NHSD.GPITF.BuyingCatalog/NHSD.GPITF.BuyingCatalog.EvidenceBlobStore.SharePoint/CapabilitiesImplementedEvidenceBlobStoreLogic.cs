using Microsoft.Extensions.Configuration;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public sealed class CapabilitiesImplementedEvidenceBlobStoreLogic : EvidenceBlobStoreLogic, ICapabilitiesImplementedEvidenceBlobStoreLogic
  {
    public CapabilitiesImplementedEvidenceBlobStoreLogic(
      IConfiguration config,
      IOrganisationsDatastore organisationsDatastore,
      ISolutionsDatastore solutionsDatastore,
      ICapabilitiesImplementedDatastore capabilitiesImplementedDatastore,
      IStandardsApplicableDatastore standardsApplicableDatastore,
      ICapabilitiesDatastore capabilitiesDatastore,
      IStandardsDatastore standardsDatastore) :
      base(
        config,
        organisationsDatastore,
        solutionsDatastore,
        capabilitiesImplementedDatastore,
        standardsApplicableDatastore,
        capabilitiesDatastore,
        standardsDatastore)
    {
    }

    protected override string GetFolderName()
    {
      return CapabilityFolderName;
    }

    protected override string GetFolderClaimName(ClaimsBase claim)
    {
      var specifiClaim = (CapabilitiesImplemented)claim;
      var cap = _capabilitiesDatastore.ById(specifiClaim.CapabilityId);

      return cap.Name;
    }

    protected override IClaimsDatastore<ClaimsBase> ClaimsDatastore
    {
      get
      {
        return (IClaimsDatastore<ClaimsBase>)_capabilitiesImplementedDatastore;
      }
    }
  }
}
