using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilitiesImplementedReviewsDatastore : DatastoreBase<CapabilitiesImplementedReviews>, ICapabilitiesImplementedReviewsDatastore
  {
    public CapabilitiesImplementedReviewsDatastore(
      IRestClientFactory crmFactory,
      ILogger<DatastoreBase<CapabilitiesImplementedReviews>> logger,
      ISyncPolicyFactory policy) :
      base(crmFactory, logger, policy)
    {
    }

    private string ResourceBase { get; } = "/CapabilitiesImplementedReviews";

    public IEnumerable<IEnumerable<CapabilitiesImplementedReviews>> ByEvidence(string evidenceId)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ByEvidence/{evidenceId}");
        var retval = GetResponse<IEnumerable<IEnumerable<CapabilitiesImplementedReviews>>>(request);

        return retval;
      });
    }

    public CapabilitiesImplementedReviews ById(string id)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ById/{id}");
        var retval = GetResponse<CapabilitiesImplementedReviews>(request);

        return retval;
      });
    }

    public CapabilitiesImplementedReviews Create(CapabilitiesImplementedReviews review)
    {
      return GetInternal(() =>
      {
        review.Id = UpdateId(review.Id);
        var request = GetPostRequest($"{ResourceBase}", review);
        var retval = GetResponse<CapabilitiesImplementedReviews>(request);

        return retval;
      });
    }

    public void Delete(CapabilitiesImplementedReviews review)
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
