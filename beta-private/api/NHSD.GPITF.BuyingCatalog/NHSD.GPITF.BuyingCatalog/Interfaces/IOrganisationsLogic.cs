using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IOrganisationsLogic
  {
    Organisations ByODS(string odsCode);
    Organisations ById(string id);
    IQueryable<Organisations> GetAll();
    Organisations Create(Organisations org);
    void Update(Organisations org);
    void Delete(Organisations org);
  }
#pragma warning restore CS1591
}
