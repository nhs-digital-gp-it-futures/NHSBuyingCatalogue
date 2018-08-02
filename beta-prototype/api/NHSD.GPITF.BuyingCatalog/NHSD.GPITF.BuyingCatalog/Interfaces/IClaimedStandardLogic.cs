using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IClaimedStandardLogic
  {
    IQueryable<ClaimedStandard> BySolution(string solutionId);
    ClaimedStandard Create(ClaimedStandard claimedstandard);
    void Update(ClaimedStandard claimedstandard);
    void Delete(ClaimedStandard claimedstandard);
  }
#pragma warning restore CS1591
}
