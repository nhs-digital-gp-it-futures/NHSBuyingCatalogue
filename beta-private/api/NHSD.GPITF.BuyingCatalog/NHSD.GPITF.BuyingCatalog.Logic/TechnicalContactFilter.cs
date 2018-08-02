using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class TechnicalContactFilter : FilterBase<TechnicalContact>, ITechnicalContactFilter
  {
    private readonly ISolutionDatastore _solutionDatastore;

    public TechnicalContactFilter(
      IHttpContextAccessor context,
      ISolutionDatastore solutionDatastore) :
      base(context)
    {
      _solutionDatastore = solutionDatastore;
    }

    protected override TechnicalContact Filter(TechnicalContact input)
    {
      if (_context.HasRole(Roles.Admin) ||
        _context.HasRole(Roles.Buyer))
      {
        return input;
      }

      // Supplier: only own TechnicalContacts
      var soln = _solutionDatastore.ById(input.SolutionId);
      return _context.ContextOrganisationId() == soln.OrganisationId ? input : null;
    }
  }
}
