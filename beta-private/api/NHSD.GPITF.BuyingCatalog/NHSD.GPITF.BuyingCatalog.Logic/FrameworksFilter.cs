using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class FrameworksFilter : FilterBase<Frameworks>, IFrameworksFilter
  {
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly IFrameworksDatastore _frameworkDatastore;

    public FrameworksFilter(
      IHttpContextAccessor context,
      ISolutionsDatastore solutionDatastore,
      IFrameworksDatastore frameworkDatastore) :
      base(context)
    {
      _solutionDatastore = solutionDatastore;
      _frameworkDatastore = frameworkDatastore;
    }

    protected override Frameworks Filter(Frameworks input)
    {
      if (!_context.HasRole(Roles.Supplier))
      {
        return input;
      }

      // Supplier: only own Solutions
      var orgId = _context.HttpContext.User.Claims
        .Where(x => x.Type == nameof(Organisations))
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
