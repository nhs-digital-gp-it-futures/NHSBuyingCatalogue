using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilitiesImplementedEvidenceDatastore : DatastoreBase<CapabilitiesImplementedEvidence>, ICapabilitiesImplementedEvidenceDatastore
  {
    public CapabilitiesImplementedEvidenceDatastore(
      IRestClientFactory crmFactory,
      ILogger<DatastoreBase<CapabilitiesImplementedEvidence>> logger,
      ISyncPolicyFactory policy) :
      base(crmFactory, logger, policy)
    {
    }

    private string ResourceBase { get; } = "/CapabilitiesImplementedEvidence";

    public IEnumerable<IEnumerable<CapabilitiesImplementedEvidence>> ByClaim(string claimId)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ByClaim/{claimId}");
        var retval = GetResponse<IEnumerable<IEnumerable<CapabilitiesImplementedEvidence>>>(request);

        return retval;
      });
    }

    public CapabilitiesImplementedEvidence ById(string id)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ById/{id}");
        var retval = GetResponse<CapabilitiesImplementedEvidence>(request);

        return retval;
      });
    }

    public CapabilitiesImplementedEvidence Create(CapabilitiesImplementedEvidence evidence)
    {
      return GetInternal(() =>
      {
        evidence.Id = UpdateId(evidence.Id);
        var request = GetPostRequest($"{ResourceBase}", evidence);
        var retval = GetResponse<CapabilitiesImplementedEvidence>(request);

        return retval;
      });
    }
  }
}
