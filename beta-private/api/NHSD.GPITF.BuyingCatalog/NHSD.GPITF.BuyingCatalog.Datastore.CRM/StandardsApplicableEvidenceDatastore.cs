using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardsApplicableEvidenceDatastore : DatastoreBase<StandardsApplicableEvidence>, IStandardsApplicableEvidenceDatastore
  {
    public StandardsApplicableEvidenceDatastore(
      IRestClientFactory crmFactory,
      ILogger<DatastoreBase<StandardsApplicableEvidence>> logger,
      ISyncPolicyFactory policy) : base(crmFactory, logger, policy)
    {
    }

    private string ResourceBase { get; } = "/StandardsApplicableEvidence";

    public IEnumerable<IEnumerable<StandardsApplicableEvidence>> ByClaim(string claimId)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ByClaim/{claimId}");
        var retval = GetResponse<IEnumerable<IEnumerable<StandardsApplicableEvidence>>>(request);

        return retval;
      });
    }

    public StandardsApplicableEvidence ById(string id)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ById/{id}");
        var retval = GetResponse<StandardsApplicableEvidence>(request);

        return retval;
      });
    }

    public StandardsApplicableEvidence Create(StandardsApplicableEvidence evidence)
    {
      return GetInternal(() =>
      {
        evidence.Id = UpdateId(evidence.Id);
        var request = GetPostRequest($"{ResourceBase}", evidence);
        var retval = GetResponse<StandardsApplicableEvidence>(request);

        return retval;
      });
    }
  }
}
