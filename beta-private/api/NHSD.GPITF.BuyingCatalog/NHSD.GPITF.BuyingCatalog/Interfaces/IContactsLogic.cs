using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IContactsLogic
  {
    Contacts ById(string id);
    IQueryable<Contacts> ByOrganisation(string organisationId);
    Contacts ByEmail(string email);
    Contacts Create(Contacts contact);
    void Update(Contacts contact);
    void Delete(Contacts contact);
  }
#pragma warning restore CS1591
}
