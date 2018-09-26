﻿using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.IO;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IEvidenceBlobStoreLogic
  {
    void PrepareForSolution(string solutionId);
    string AddEvidenceForClaim(string claimId, Stream file, string filename, string subFolder = null);
    IEnumerable<BlobInfo> EnumerateFolder(string claimId, string subFolder = null);
  }
#pragma warning restore CS1591
}
