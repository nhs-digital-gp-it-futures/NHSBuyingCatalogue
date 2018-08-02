using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class OrganisationDatastore : DatastoreBase<Organisation>, IOrganisationDatastore
  {
    public OrganisationDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<OrganisationDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public Organisation ById(string id)
    {
      throw new NotImplementedException();
    }

    public Organisation ByODS(string odsCode)
    {
      throw new NotImplementedException();
    }

    public Organisation Create(Organisation org)
    {
      throw new NotImplementedException();
    }

    public void Delete(Organisation org)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Organisation> GetAll()
    {
      throw new NotImplementedException();
    }

    public void Update(Organisation org)
    {
      throw new NotImplementedException();
    }
  }
}
