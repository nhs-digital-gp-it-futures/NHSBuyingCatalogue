using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database.Porcelain
{
  public sealed class SearchDatastore : DatastoreBase<SolutionEx>, ISearchDatastore
  {
    private readonly IFrameworksDatastore _frameworkDatastore;
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ICapabilitiesDatastore _capabilityDatastore;
    private readonly ICapabilitiesImplementedDatastore _claimedCapabilityDatastore;
    private readonly ISolutionsExDatastore _solutionExDatastore;

    public SearchDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<DatastoreBase<SolutionEx>> logger,
      ISyncPolicyFactory policy,
      IFrameworksDatastore frameworkDatastore,
      ISolutionsDatastore solutionDatastore,
      ICapabilitiesDatastore capabilityDatastore,
      ICapabilitiesImplementedDatastore claimedCapabilityDatastore,
      ISolutionsExDatastore solutionExDatastore) :
      base(dbConnectionFactory, logger, policy)
    {
      _frameworkDatastore = frameworkDatastore;
      _solutionDatastore = solutionDatastore;
      _capabilityDatastore = capabilityDatastore;
      _claimedCapabilityDatastore = claimedCapabilityDatastore;
      _solutionExDatastore = solutionExDatastore;
    }

    public IEnumerable<SolutionEx> SolutionExByKeyword(string keyword)
    {
      // get all Solutions via frameworks
      var allSolns = _frameworkDatastore.GetAll()
        .SelectMany(fw => _solutionDatastore.ByFramework(fw.Id));

      // get all Solutions with keyword in name or description
      var allSolnsKeywordIds = allSolns
        .Where(soln =>
          soln.Name.ToLowerInvariant().Contains(keyword.ToLowerInvariant()) ||
          soln.Description.ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
        .Select(soln => soln.Id);

      // get all Capabilities with keyword
      var allCapsKeywordIds = _capabilityDatastore.GetAll()
        .Where(cap =>
          cap.Name.ToLowerInvariant().Contains(keyword.ToLowerInvariant()) ||
          cap.Description.ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
        .Select(cap => cap.Id);

      // get all Solutions with at least one ClaimedCapability with keyword
      var allSolnsClaimedCapsIds = allSolns
        .Where(soln => _claimedCapabilityDatastore
          .BySolution(soln.Id)
          .Select(cc => cc.CapabilityId)
          .Intersect(allCapsKeywordIds)
          .Any())
        .Select(soln => soln.Id)
        .Distinct();

      // unique set of Solutions with keyword in name/description or ClaimedCapability
      var uniqueSolnIds = allSolnsKeywordIds
        .Union(allSolnsClaimedCapsIds)
        .Distinct();

      return uniqueSolnIds.Select(solnId => _solutionExDatastore.BySolution(solnId));
    }
  }
}
