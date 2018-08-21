using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface ISolutionsLogic
  {
    IQueryable<Solutions> ByFramework(string frameworkId);
    Solutions ById(string id);
    IQueryable<Solutions> ByOrganisation(string organisationId);
    Solutions Create(Solutions solution);
    void Update(Solutions solution);
    void Delete(Solutions solution);
  }
#pragma warning restore CS1591
}
