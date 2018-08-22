using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ReviewsFilter : FilterBase<Reviews>, IReviewsFilter
  {
    private readonly ISolutionsDatastore _solutionDatastore;

    public ReviewsFilter(
      IHttpContextAccessor context,
      ISolutionsDatastore solutionDatastore) :
      base(context)
    {
      _solutionDatastore = solutionDatastore;
    }

    protected override Reviews Filter(Reviews input)
    {
      if (_context.HasRole(Roles.Admin))
      {
        return input;
      }

      if (_context.HasRole(Roles.Buyer))
      {
        // Buyer: no visibility
        return null;
      }

      // Supplier: only own messages
      var soln = _solutionDatastore.ById(input.SolutionId);
      return _context.OrganisationId() == soln?.OrganisationId ? input : null;
    }
  }
}
