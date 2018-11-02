using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardsApplicableReviewsDatastore : DatastoreBase<StandardsApplicableReviews>, IStandardsApplicableReviewsDatastore
  {
    public StandardsApplicableReviewsDatastore(
      IRestClientFactory crmFactory,
      ILogger<DatastoreBase<StandardsApplicableReviews>> logger,
      ISyncPolicyFactory policy) :
      base(crmFactory, logger, policy)
    {
    }

    private string ResourceBase { get; } = "/StandardsApplicableReviews";

    public IEnumerable<IEnumerable<StandardsApplicableReviews>> ByEvidence(string evidenceId)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ByEvidence/{evidenceId}");
        var retval = GetResponse<IEnumerable<IEnumerable<StandardsApplicableReviews>>>(request);

        return retval;
      });
    }

    public StandardsApplicableReviews ById(string id)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ById/{id}");
        var retval = GetResponse<StandardsApplicableReviews>(request);

        return retval;
      });
    }

    public StandardsApplicableReviews Create(StandardsApplicableReviews review)
    {
      return GetInternal(() =>
      {
        review.Id = UpdateId(review.Id);
        var request = GetPostRequest($"{ResourceBase}", review);
        var retval = GetResponse<StandardsApplicableReviews>(request);

        return retval;
      });
    }

    public void Delete(StandardsApplicableReviews review)
    {
      GetInternal(() =>
      {
        var request = GetDeleteRequest($"{ResourceBase}", review);
        var resp = GetRawResponse(request);

        return 0;
      });
    }
  }
}
