using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class OrganisationsDatastore : DatastoreBase<Organisations>, IOrganisationsDatastore
  {
    public OrganisationsDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<OrganisationsDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public Organisations ById(string id)
    {
      throw new NotImplementedException();
    }

    public Organisations ByODS(string odsCode)
    {
      throw new NotImplementedException();
    }

    public Organisations Create(Organisations org)
    {
      throw new NotImplementedException();
    }

    public void Delete(Organisations org)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Organisations> GetAll()
    {
      throw new NotImplementedException();
    }

    public void Update(Organisations org)
    {
      throw new NotImplementedException();
    }
  }
}
