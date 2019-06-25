using Gif.Service.Models;
using System.Collections.Generic;

namespace Gif.Service.Contracts
{
#pragma warning disable CS1591
  public interface IOrganisationsDatastore
  {
    Organisation ByContact(string contactId);
    Organisation ById(string organisationId);
    IEnumerable<Organisation> GetAll();
  }
#pragma warning restore CS1591
}
