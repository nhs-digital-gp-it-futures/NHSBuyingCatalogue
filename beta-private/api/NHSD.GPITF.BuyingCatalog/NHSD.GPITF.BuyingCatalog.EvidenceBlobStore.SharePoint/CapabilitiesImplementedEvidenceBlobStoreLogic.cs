using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public sealed class CapabilitiesImplementedEvidenceBlobStoreLogic : ICapabilitiesImplementedEvidenceBlobStoreLogic
  {
    public string AddEvidenceForClaim(string claimId, Stream file, string filename, string subFolder = null)
    {
      return "TODO";
    }

    public IEnumerable<BlobInfo> EnumerateFolder(string claimId, string subFolder = null)
    {
      return Enumerable.Empty<BlobInfo>();
    }

    public void PrepareForSolution(string solutionId)
    {
    }
  }

  public sealed class StandardsApplicableEvidenceBlobStoreLogic : IStandardsApplicableEvidenceBlobStoreLogic
  {
    public string AddEvidenceForClaim(string claimId, Stream file, string filename, string subFolder = null)
    {
      return "TODO";
    }

    public IEnumerable<BlobInfo> EnumerateFolder(string claimId, string subFolder = null)
    {
      return Enumerable.Empty<BlobInfo>();
    }

    public void PrepareForSolution(string solutionId)
    {
    }
  }
}
