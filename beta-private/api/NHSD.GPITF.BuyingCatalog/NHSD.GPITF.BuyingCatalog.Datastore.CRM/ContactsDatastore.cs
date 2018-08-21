using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class ContactsDatastore : DatastoreBase<Contacts>, IContactsDatastore
  {
    public ContactsDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<ContactsDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public Contacts ByEmail(string email)
    {
      throw new NotImplementedException();
    }

    public Contacts ById(string id)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Contacts> ByOrganisation(string organisationId)
    {
      throw new NotImplementedException();
    }
  }
}
