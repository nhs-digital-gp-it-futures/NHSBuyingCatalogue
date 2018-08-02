using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain
{
  public sealed class CapabilityMappingsDatastore : DatastoreBase<CapabilityMappings>, ICapabilityMappingsDatastore
  {
    public CapabilityMappingsDatastore(
      IRestClientFactory crmConnectionFactory, 
      ILogger<CapabilityMappingsDatastore> logger, 
      ISyncPolicyFactory policy) : 
      base(crmConnectionFactory, logger, policy)
    {
    }

    public CapabilityMappings GetAll()
    {
      throw new NotImplementedException();
    }
  }
}
