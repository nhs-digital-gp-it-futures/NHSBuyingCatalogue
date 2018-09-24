using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

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

  public interface IEvidenceBlobDatastore
  {
    void PrepareForSolution(string solutionId);
  }

  public interface ICapabilitiesImplementedEvidenceBlobDatastore : IEvidenceBlobDatastore
  {
  }

  public interface IStandardsApplicbleEvidenceBlobDatastore : IEvidenceBlobDatastore
  {
  }
#pragma warning restore CS1591
}
