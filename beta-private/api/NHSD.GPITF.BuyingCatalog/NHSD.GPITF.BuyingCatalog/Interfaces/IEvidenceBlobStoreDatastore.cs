using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.IO;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IEvidenceBlobStoreDatastore
  {
    void PrepareForSolution(IClaimsInfoProvider claimsInfoProvider, string solutionId);
    string AddEvidenceForClaim(IClaimsInfoProvider claimsInfoProvider, string claimId, Stream file, string filename, string subFolder = null);
    Stream GetFileStream(IClaimsInfoProvider claimsInfoProvider, string claimId, string extUrl);
    IEnumerable<BlobInfo> EnumerateFolder(IClaimsInfoProvider claimsInfoProvider, string claimId, string subFolder = null);
  }
#pragma warning restore CS1591
}
