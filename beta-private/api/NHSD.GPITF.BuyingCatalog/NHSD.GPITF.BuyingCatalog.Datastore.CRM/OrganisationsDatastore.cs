using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;

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

    public Organisations ByContact(string contactId)
    {
      throw new NotImplementedException();
    }
  }
}
