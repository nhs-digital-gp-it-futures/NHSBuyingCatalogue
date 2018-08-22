using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilitiesDatastore : DatastoreBase<Capabilities>, ICapabilitiesDatastore
  {
    public CapabilitiesDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<CapabilitiesDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Capabilities> ByFramework(string frameworkId)
    {
      throw new NotImplementedException();
    }

    public Capabilities ById(string id)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Capabilities> ByIds(IEnumerable<string> ids)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Capabilities> ByStandard(string standardId, bool isOptional)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Capabilities> GetAll()
    {
      throw new NotImplementedException();
    }
  }
}
