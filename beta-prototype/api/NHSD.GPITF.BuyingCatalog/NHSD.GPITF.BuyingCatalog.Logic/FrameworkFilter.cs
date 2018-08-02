using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class FrameworkFilter : FilterBase<Framework>, IFrameworkFilter
  {
    private readonly ISolutionDatastore _solutionDatastore;
    private readonly IFrameworkDatastore _frameworkDatastore;

    public FrameworkFilter(
      IHttpContextAccessor context,
      ISolutionDatastore solutionDatastore,
      IFrameworkDatastore frameworkDatastore) :
      base(context)
    {
      _solutionDatastore = solutionDatastore;
      _frameworkDatastore = frameworkDatastore;
    }

    protected override Framework Filter(Framework input)
    {
      if (!_context.HasRole(Roles.Supplier))
      {
        return input;
      }

      // Supplier: only own Solutions
      var orgId = _context.HttpContext.User.Claims
        .Where(x => x.Type == nameof(Organisation))
        .Select(x => x.Value)
        .SingleOrDefault();
      if (orgId is null)
      {
        return null;
      }

      var frameworkIds = _solutionDatastore
        .ByOrganisation(orgId)
        .SelectMany(soln => _frameworkDatastore.BySolution(soln.Id))
        .Select(fw => fw.Id);

      return frameworkIds.Contains(input.Id) ? input : null;
    }
  }
}
