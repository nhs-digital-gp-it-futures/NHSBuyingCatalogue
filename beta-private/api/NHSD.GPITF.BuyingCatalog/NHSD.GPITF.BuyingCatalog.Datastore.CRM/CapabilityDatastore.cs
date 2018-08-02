using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class CapabilityDatastore : DatastoreBase<Capability>, ICapabilityDatastore
  {
    public CapabilityDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<CapabilityDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Capability> ByFramework(string frameworkId)
    {
      throw new NotImplementedException();
    }

    public Capability ById(string id)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Capability> ByIds(IEnumerable<string> ids)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Capability> ByStandard(string standardId, bool isOptional)
    {
      throw new NotImplementedException();
    }

    public Capability Create(Capability capability)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Capability> GetAll()
    {
      throw new NotImplementedException();
    }

    public void Update(Capability capability)
    {
      throw new NotImplementedException();
    }
  }
}
