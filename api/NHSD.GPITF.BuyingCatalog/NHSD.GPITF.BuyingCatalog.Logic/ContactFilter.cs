using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ContactFilter : FilterBase<Contact>, IContactFilter
  {
    private readonly IOrganisationDatastore _organisationDatastore;

    public ContactFilter(
      IHttpContextAccessor context,
      IOrganisationDatastore organisationDatastore) :
      base(context)
    {
      _organisationDatastore = organisationDatastore;
    }

    protected override Contact Filter(Contact input)
    {
      if (!_context.HasRole(Roles.Supplier))
      {
        return input;
      }

      // Supplier: only own Contacts+NHSD
      var orgId = _context.HttpContext.User.Claims
        .Where(x => x.Type == nameof(Organisation))
        .Select(x => x.Value)
        .SingleOrDefault();
      if (orgId is null)
      {
        return null;
      }

      var contactOrg = _organisationDatastore.ById(input.OrganisationId);

      return (input.OrganisationId == orgId ||
        contactOrg.PrimaryRoleId == PrimaryRole.GovernmentDepartment)
        ? input : null;
    }
  }
}
