using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public sealed class StandardsApplicableEvidenceBlobStoreLogic : EvidenceBlobStoreLogic, IStandardsApplicableEvidenceBlobStoreLogic
  {
    public StandardsApplicableEvidenceBlobStoreLogic(
      IEvidenceBlobStoreDatastore evidenceBlobStoreDatastore,
      ICapabilitiesImplementedDatastore capabilitiesImplementedDatastore,
      IStandardsApplicableDatastore standardsApplicableDatastore,
      ICapabilitiesDatastore capabilitiesDatastore,
      IStandardsDatastore standardsDatastore,
      IEvidenceBlobStoreValidator validator,
      IStandardsApplicableEvidenceBlobStoreValidator claimValidator) :
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
      return StandardsFolderName;
    }

    public override string GetFolderClaimName(ClaimsBase claim)
    {
      var specifiClaim = (StandardsApplicable)claim;
      var std = _standardsDatastore.ById(specifiClaim.StandardId);

      return std.Name;
    }

    protected override IClaimsDatastore<ClaimsBase> ClaimsDatastore
    {
      get
      {
        return (IClaimsDatastore<ClaimsBase>)_standardsApplicableDatastore;
      }
    }
  }
}
