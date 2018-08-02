using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IFrameworkLogic
  {
    IQueryable<Framework> GetAll();
    void Update(Framework framework);
    Framework Create(Framework framework);
    IQueryable<Framework> BySolution(string solutionId);
    IQueryable<Framework> ByStandard(string standardId);
    Framework ById(string id);
    IQueryable<Framework> ByCapability(string capabilityId);
  }
#pragma warning restore CS1591
}
