using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IFrameworkDatastore
  {
    IQueryable<Framework> ByCapability(string capabilityId);
    Framework ById(string id);
    IQueryable<Framework> BySolution(string solutionId);
    IQueryable<Framework> ByStandard(string standardId);
    Framework Create(Framework framework);
    IQueryable<Framework> GetAll();
    void Update(Framework framework);
  }
#pragma warning restore CS1591
}
