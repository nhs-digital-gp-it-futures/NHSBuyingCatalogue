using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public sealed class CapabilitiesImplementedEvidenceBlobStoreLogic : EvidenceBlobStoreLogic, ICapabilitiesImplementedEvidenceBlobStoreLogic
  {
    public CapabilitiesImplementedEvidenceBlobStoreLogic(
      IEvidenceBlobStoreDatastore evidenceBlobStoreDatastore,
      ICapabilitiesImplementedDatastore capabilitiesImplementedDatastore,
      IStandardsApplicableDatastore standardsApplicableDatastore,
      ICapabilitiesDatastore capabilitiesDatastore,
      IStandardsDatastore standardsDatastore,
      IEvidenceBlobStoreValidator validator,
      ICapabilitiesImplementedEvidenceBlobStoreValidator claimValidator) :
      base(
        evidenceBlobStoreDatastore,
        capabilitiesImplementedDatastore,
        standardsApplicableDatastore,
        capabilitiesDatastore,
        standardsDatastore,
        validator,
        claimValidator)
    {
    }

    public override string GetFolderName()
    {
      return CapabilityFolderName;
    }

    public override string GetFolderClaimName(ClaimsBase claim)
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
