using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IClaimedCapabilityLogic
  {
    IQueryable<ClaimedCapability> BySolution(string solutionId);
    ClaimedCapability Create(ClaimedCapability claimedcapability);
    void Update(ClaimedCapability claimedcapability);
    void Delete(ClaimedCapability claimedcapability);
  }
#pragma warning restore CS1591
}
