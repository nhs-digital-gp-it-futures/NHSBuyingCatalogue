using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class ContactDatastore : DatastoreBase<Contact>, IContactDatastore
  {
    public ContactDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<ContactDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public Contact ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Contact>(id);
      });
    }

    public IQueryable<Contact> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Contact>().Where(c => c.OrganisationId == organisationId).AsQueryable();
      });
    }

    public Contact ByEmail(string email)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Contact>().SingleOrDefault(c => c.EmailAddress1.ToLowerInvariant() == email.ToLowerInvariant());
      });
    }

    public Contact Create(Contact contact)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          contact.Id = Guid.NewGuid().ToString();
          _dbConnection.Value.Insert(contact, trans);
          trans.Commit();

          return contact;
        }
      });
    }

    public void Update(Contact contact)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(contact, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void Delete(Contact contact)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Delete(contact, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
