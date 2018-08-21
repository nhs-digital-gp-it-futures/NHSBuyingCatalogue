using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ICapabilitiesImplementedLogic
  {
    IQueryable<CapabilitiesImplemented> BySolution(string solutionId);
    CapabilitiesImplemented Create(CapabilitiesImplemented claimedcapability);
    void Update(CapabilitiesImplemented claimedcapability);
    void Delete(CapabilitiesImplemented claimedcapability);
  }
#pragma warning restore CS1591
}
