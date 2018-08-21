using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ContactsFilter : FilterBase<Contacts>, IContactsFilter
  {
    private readonly IOrganisationsDatastore _organisationDatastore;

    public ContactsFilter(
      IHttpContextAccessor context,
      IOrganisationsDatastore organisationDatastore) :
      base(context)
    {
      _organisationDatastore = organisationDatastore;
    }

    protected override Contacts Filter(Contacts input)
    {
      if (!_context.HasRole(Roles.Supplier))
      {
        return input;
      }

      // Supplier: only own Contacts+NHSD
      var orgId = _context.HttpContext.User.Claims
        .Where(x => x.Type == nameof(Organisations))
        .Select(x => x.Value)
        .SingleOrDefault();
      if (orgId is null)
      {
        return null;
      }

      // TODO   change to use IOrganisationsDatastore.ByEmail
      var contactOrg = _organisationDatastore.ByODS(input.OrganisationId);

      return (input.OrganisationId == orgId ||
        contactOrg.PrimaryRoleId == PrimaryRole.GovernmentDepartment)
        ? input : null;
    }
  }
}
