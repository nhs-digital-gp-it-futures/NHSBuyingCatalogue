using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class ContactDatastore : DatastoreBase<Contact>, IContactDatastore
  {
    public ContactDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<ContactDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public Contact ByEmail(string email)
    {
      throw new NotImplementedException();
    }

    public Contact ById(string id)
    {
      throw new NotImplementedException();
    }

    public IQueryable<Contact> ByOrganisation(string organisationId)
    {
      throw new NotImplementedException();
    }

    public Contact Create(Contact contact)
    {
      throw new NotImplementedException();
    }

    public void Delete(Contact contact)
    {
      throw new NotImplementedException();
    }

    public void Update(Contact contact)
    {
      throw new NotImplementedException();
    }
  }
}
