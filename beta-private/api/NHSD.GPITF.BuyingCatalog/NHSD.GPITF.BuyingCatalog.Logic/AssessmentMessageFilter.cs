using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class AssessmentMessageFilter : FilterBase<AssessmentMessage>, IAssessmentMessageFilter
  {
    private readonly ISolutionDatastore _solutionDatastore;

    public AssessmentMessageFilter(
      IHttpContextAccessor context,
      ISolutionDatastore solutionDatastore) :
      base(context)
    {
      _solutionDatastore = solutionDatastore;
    }

    protected override AssessmentMessage Filter(AssessmentMessage input)
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
      return _context.ContextOrganisationId() == soln?.OrganisationId ? input : null;
    }
  }
}
