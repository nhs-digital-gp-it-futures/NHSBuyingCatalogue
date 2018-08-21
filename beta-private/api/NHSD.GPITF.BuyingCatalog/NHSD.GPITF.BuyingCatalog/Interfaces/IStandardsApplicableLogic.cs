using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IStandardsApplicableLogic
  {
    IQueryable<StandardsApplicable> BySolution(string solutionId);
    StandardsApplicable Create(StandardsApplicable claimedstandard);
    void Update(StandardsApplicable claimedstandard);
    void Delete(StandardsApplicable claimedstandard);
  }
#pragma warning restore CS1591
}
