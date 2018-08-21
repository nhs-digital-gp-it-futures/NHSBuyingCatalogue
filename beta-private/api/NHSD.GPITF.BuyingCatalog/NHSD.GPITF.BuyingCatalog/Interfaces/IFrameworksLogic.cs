using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IFrameworksLogic
  {
    IQueryable<Frameworks> GetAll();
    void Update(Frameworks framework);
    Frameworks Create(Frameworks framework);
    IQueryable<Frameworks> BySolution(string solutionId);
    IQueryable<Frameworks> ByStandard(string standardId);
    Frameworks ById(string id);
    IQueryable<Frameworks> ByCapability(string capabilityId);
  }
#pragma warning restore CS1591
}
