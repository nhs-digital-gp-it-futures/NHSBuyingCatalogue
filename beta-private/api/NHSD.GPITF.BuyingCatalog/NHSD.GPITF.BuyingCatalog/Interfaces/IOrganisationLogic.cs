using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IOrganisationLogic
  {
    Organisation ByODS(string odsCode);
    Organisation ById(string id);
    IQueryable<Organisation> GetAll();
    Organisation Create(Organisation org);
    void Update(Organisation org);
    void Delete(Organisation org);
  }
#pragma warning restore CS1591
}
