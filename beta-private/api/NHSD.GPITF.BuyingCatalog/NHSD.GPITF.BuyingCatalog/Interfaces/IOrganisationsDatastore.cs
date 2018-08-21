using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IOrganisationsDatastore
  {
    Organisations ByODS(string odsCode);
    IQueryable<Organisations> GetAll();
    Organisations ById(string id);
    Organisations Create(Organisations org);
    void Update(Organisations org);
    void Delete(Organisations org);
  }
#pragma warning restore CS1591
}
