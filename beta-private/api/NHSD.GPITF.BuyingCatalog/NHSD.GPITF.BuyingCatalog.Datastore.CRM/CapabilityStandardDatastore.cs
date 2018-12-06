using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilityStandardDatastore : DatastoreBase<CapabilityStandard>, ICapabilityStandardDatastore
  {
    public CapabilityStandardDatastore(
      IRestClientFactory crmFactory,
      ILogger<DatastoreBase<CapabilityStandard>> logger,
      ISyncPolicyFactory policy) :
      base(crmFactory, logger, policy)
    {
    }

    private string ResourceBase { get; } = "/CapabilityStandards";

    public IEnumerable<CapabilityStandard> GetAll()
    {
      return GetInternal(() =>
      {
        var request = GetAllRequest($"{ResourceBase}");
        var retval = GetResponse<PaginatedList<CapabilityStandard>>(request);

        return retval.Items;
      });
    }
  }
}
