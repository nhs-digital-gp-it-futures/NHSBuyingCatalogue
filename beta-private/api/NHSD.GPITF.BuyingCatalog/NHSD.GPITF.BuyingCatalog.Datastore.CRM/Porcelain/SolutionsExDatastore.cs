using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain
{
  public sealed class SolutionsExDatastore : DatastoreBase<SolutionEx>, ISolutionsExDatastore
  {
    private string ResourceBase { get; } = "/porcelain/SolutionsEx/Update";

    private readonly ISolutionsDatastore _solutionDatastore;
    private readonly ITechnicalContactsDatastore _technicalContactDatastore;

    private readonly ICapabilitiesImplementedDatastore _claimedCapabilityDatastore;
    private readonly ICapabilitiesImplementedEvidenceDatastore _claimedCapabilityEvidenceDatastore;
    private readonly ICapabilitiesImplementedReviewsDatastore _claimedCapabilityReviewsDatastore;

    private readonly IStandardsApplicableDatastore _claimedStandardDatastore;
    private readonly IStandardsApplicableEvidenceDatastore _claimedStandardEvidenceDatastore;
    private readonly IStandardsApplicableReviewsDatastore _claimedStandardReviewsDatastore;

    private readonly ILinkManagerDatastore _linkManagerDatastore;
    private readonly IFrameworksDatastore _frameworksDatastore;

    public SolutionsExDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<SolutionsExDatastore> logger,
      ISyncPolicyFactory policy,

      ISolutionsDatastore solutionDatastore,
      ITechnicalContactsDatastore technicalContactDatastore,

      ICapabilitiesImplementedDatastore claimedCapabilityDatastore,
      ICapabilitiesImplementedEvidenceDatastore claimedCapabilityEvidenceDatastore,
      ICapabilitiesImplementedReviewsDatastore claimedCapabilityReviewsDatastore,

      IStandardsApplicableDatastore claimedStandardDatastore,
      IStandardsApplicableEvidenceDatastore claimedStandardEvidenceDatastore,
      IStandardsApplicableReviewsDatastore claimedStandardReviewsDatastore,

      ILinkManagerDatastore linkManagerDatastore,
      IFrameworksDatastore frameworksDatastore
      ) :
      base(crmConnectionFactory, logger, policy)
    {
      _solutionDatastore = solutionDatastore;
      _technicalContactDatastore = technicalContactDatastore;

      _claimedCapabilityDatastore = claimedCapabilityDatastore;
      _claimedCapabilityEvidenceDatastore = claimedCapabilityEvidenceDatastore;
      _claimedCapabilityReviewsDatastore = claimedCapabilityReviewsDatastore;

      _claimedStandardDatastore = claimedStandardDatastore;
      _claimedStandardEvidenceDatastore = claimedStandardEvidenceDatastore;
      _claimedStandardReviewsDatastore = claimedStandardReviewsDatastore;

      _linkManagerDatastore = linkManagerDatastore;
      _frameworksDatastore = frameworksDatastore;
    }

    public SolutionEx BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var retval = new SolutionEx
        {
          Solution = _solutionDatastore.ById(solutionId),
          TechnicalContact = _technicalContactDatastore.BySolution(solutionId).ToList(),
          ClaimedCapability = _claimedCapabilityDatastore.BySolution(solutionId).ToList(),
          ClaimedStandard = _claimedStandardDatastore.BySolution(solutionId).ToList()
        };

        // populate Evidence + Review
        retval.ClaimedCapabilityEvidence = retval.ClaimedCapability
          .SelectMany(cc => _claimedCapabilityEvidenceDatastore.ByClaim(cc.Id))
            .SelectMany(x => x)
            .ToList();
        retval.ClaimedCapabilityReview = retval.ClaimedCapabilityEvidence
          .SelectMany(cce => _claimedCapabilityReviewsDatastore.ByEvidence(cce.Id))
            .SelectMany(x => x)
            .ToList();
        retval.ClaimedStandardEvidence = retval.ClaimedStandard
          .SelectMany(cs => _claimedStandardEvidenceDatastore.ByClaim(cs.Id))
            .SelectMany(x => x)
            .ToList();
        retval.ClaimedStandardReview = retval.ClaimedStandardEvidence
          .SelectMany(cse => _claimedStandardReviewsDatastore.ByEvidence(cse.Id))
            .SelectMany(x => x)
            .ToList();

        return retval;
      });
    }

    public void Update(SolutionEx solnEx)
    {
      GetInternal(() =>
      {
        var fw = _frameworksDatastore.BySolution(solnEx.Solution.Id).ToList().Single();
        try
        {
          var request = GetPutRequest($"{ResourceBase}", solnEx);
          var resp = GetRawResponse(request);
        }
        finally
        {
          _linkManagerDatastore.FrameworkSolutionCreate(fw.Id, solnEx.Solution.Id);
        }

        return 0;
      });
    }
  }
}
