using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database.Porcelain
{
  public sealed class SearchDatastore : DatastoreBase<SearchResult>, ISearchDatastore
  {
    private readonly IFrameworksDatastore _frameworkDatastore;
    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ICapabilitiesDatastore _capabilityDatastore;
    private readonly ICapabilitiesImplementedDatastore _claimedCapabilityDatastore;
    private readonly ISolutionsExDatastore _solutionExDatastore;

    public SearchDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<SearchDatastore> logger,
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

    public IEnumerable<SearchResult> ByKeyword(string keyword)
    {
      // get all Solutions via frameworks
      var allSolns = _frameworkDatastore.GetAll()
        .SelectMany(fw => _solutionDatastore.ByFramework(fw.Id));

      // get all Capabilities with keyword
      var allCapsKeywordIds = _capabilityDatastore.GetAll()
        .Where(cap =>
          cap.Name.ToLowerInvariant().Contains(keyword.ToLowerInvariant()) ||
          cap.Description.ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
        .Select(cap => cap.Id);

      // get all unique Solutions with at least one ClaimedCapability with keyword
      var allSolnsCapsKeyword = allSolns
        .Where(soln => _claimedCapabilityDatastore
          .BySolution(soln.Id)
          .Select(cc => cc.CapabilityId)
          .Intersect(allCapsKeywordIds)
          .Any())
        .Distinct();

      var searchResults = allSolnsCapsKeyword.Select(soln =>
        new SearchResult
        {
          SolutionEx = _solutionExDatastore.BySolution(soln.Id),
          Distance = _claimedCapabilityDatastore.BySolution(soln.Id).Count() - allCapsKeywordIds.Count()
        });

      return searchResults;
    }
  }
}
