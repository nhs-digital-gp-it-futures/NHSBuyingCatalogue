using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IClaimsInfoProvider
  {
    ClaimsBase GetClaimById(string claimId);
    string GetFolderName();
    string GetFolderClaimName(ClaimsBase claim);
    string GetCapabilityFolderName();
    string GetStandardsFolderName();
  }
#pragma warning restore CS1591
}
