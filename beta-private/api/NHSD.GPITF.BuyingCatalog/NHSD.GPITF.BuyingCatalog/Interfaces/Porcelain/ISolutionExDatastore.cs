using NHSD.GPITF.BuyingCatalog.Models.Porcelain;

namespace NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain
{
#pragma warning disable CS1591
  public interface ISolutionExDatastore
  {
    SolutionEx BySolution(string solutionId);
    void Update(SolutionEx solnEx);
  }
#pragma warning restore CS1591
}
