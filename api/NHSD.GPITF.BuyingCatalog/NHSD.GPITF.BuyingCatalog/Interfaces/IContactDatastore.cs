using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IContactDatastore
  {
    Contact ById(string id);
    IQueryable<Contact> ByOrganisation(string organisationId);
    Contact ByEmail(string email);
    Contact Create(Contact contact);
    void Update(Contact contact);
    void Delete(Contact contact);
  }
#pragma warning restore CS1591
}
