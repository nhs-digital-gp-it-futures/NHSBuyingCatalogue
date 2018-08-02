using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IOrganisationDatastore
  {
    Organisation ByODS(string odsCode);
    IQueryable<Organisation> GetAll();
    Organisation ById(string id);
    Organisation Create(Organisation org);
    void Update(Organisation org);
    void Delete(Organisation org);
  }
#pragma warning restore CS1591
}
