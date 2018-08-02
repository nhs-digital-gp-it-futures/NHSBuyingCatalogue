using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ISolutionLogic
  {
    IQueryable<Solution> ByFramework(string frameworkId);
    Solution ById(string id);
    IQueryable<Solution> ByOrganisation(string organisationId);
    Solution Create(Solution solution);
    void Update(Solution solution);
    void Delete(Solution solution);
  }
#pragma warning restore CS1591
}
