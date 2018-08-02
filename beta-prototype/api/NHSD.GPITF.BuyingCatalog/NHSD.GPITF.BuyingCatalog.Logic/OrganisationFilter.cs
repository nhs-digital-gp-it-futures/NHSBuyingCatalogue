using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class OrganisationFilter : FilterBase<Organisation>, IOrganisationFilter
  {
    public OrganisationFilter(IHttpContextAccessor context) :
      base(context)
    {
    }

    protected override Organisation Filter(Organisation input)
    {
      if (_context.HasRole(Roles.Supplier))
      {
        // Supplier: everything except other Supplier
        return input.PrimaryRoleId != PrimaryRole.ApplicationServiceProvider || _context.ContextOrganisationId() == input.Id ? input : null;
      }
      return input;
    }
  }
}
