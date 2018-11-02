using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilitiesImplementedDatastore : DatastoreBase<CapabilitiesImplemented>, ICapabilitiesImplementedDatastore, IClaimsDatastore<ClaimsBase>
  {
    public CapabilitiesImplementedDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<CapabilitiesImplementedDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    private string ResourceBase { get; } = "/CapabilitiesImplemented";

    public CapabilitiesImplemented ById(string id)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ById/{id}");
        var retval = GetResponse<CapabilitiesImplemented>(request);

        return retval;
      });
    }

    public IEnumerable<CapabilitiesImplemented> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var request = GetAllRequest($"{ResourceBase}/BySolution/{solutionId}");
        var retval = GetResponse<PaginatedList<CapabilitiesImplemented>>(request);

        return retval.Items;
      });
    }

    public CapabilitiesImplemented Create(CapabilitiesImplemented claimedcapability)
    {
      return GetInternal(() =>
      {
        var request = GetPostRequest($"{ResourceBase}", claimedcapability);
        var retval = GetResponse<CapabilitiesImplemented>(request);

        return retval;
      });
    }

    public void Delete(CapabilitiesImplemented claimedcapability)
    {
      GetInternal(() =>
      {
        var request = GetDeleteRequest($"{ResourceBase}", claimedcapability);
        var resp = GetRawResponse(request);

        return 0;
      });
    }

    public void Update(CapabilitiesImplemented claimedcapability)
    {
      GetInternal(() =>
      {
        var request = GetPutRequest($"{ResourceBase}", claimedcapability);
        var resp = GetRawResponse(request);

        return 0;
      });
    }

    ClaimsBase IClaimsDatastore<ClaimsBase>.ById(string id)
    {
      return ById(id);
    }

    IEnumerable<ClaimsBase> IClaimsDatastore<ClaimsBase>.BySolution(string solutionId)
    {
      return BySolution(solutionId);
    }

    ClaimsBase IClaimsDatastore<ClaimsBase>.Create(ClaimsBase claim)
    {
      return Create((CapabilitiesImplemented)claim);
    }

    void IClaimsDatastore<ClaimsBase>.Delete(ClaimsBase claim)
    {
      Delete((CapabilitiesImplemented)claim);
    }

    void IClaimsDatastore<ClaimsBase>.Update(ClaimsBase claim)
    {
      Update((CapabilitiesImplemented)claim);
    }
  }
}
