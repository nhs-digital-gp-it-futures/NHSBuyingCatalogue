using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.IO;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ISolutionsDatastore
  {
    IEnumerable<Solutions> ByFramework(string frameworkId);
    Solutions ById(string id);
    IEnumerable<Solutions> ByOrganisation(string organisationId);
    Solutions Create(Solutions solution);
    void Update(Solutions solution);
  }

  public interface IEvidenceBlobStore
  {
    void PrepareForSolution(string solutionId);

    /// <summary>
    /// Upload a file to support a claim
    /// </summary>
    /// <param name="claimId">unique identifier of solution claim</param>
    /// <param name="file">Stream representing file to be uploaded</param>
    /// <param name="filename">name of file on the server</param>
    /// <param name="subFolder">optional sub-folder under claim</param>
    /// <returns>externally accessible URL of file</returns>
    string AddEvidenceForClaim(string claimId, Stream file, string filename, string subFolder = null);

//API to list folders(paged)
//API to list files(paged)
//API to download file
//API to upload file
  }

  public interface ICapabilitiesImplementedEvidenceBlobStore : IEvidenceBlobStore
  {
  }

  public interface IStandardsApplicbleEvidenceBlobStore : IEvidenceBlobStore
  {
  }
#pragma warning restore CS1591
}
