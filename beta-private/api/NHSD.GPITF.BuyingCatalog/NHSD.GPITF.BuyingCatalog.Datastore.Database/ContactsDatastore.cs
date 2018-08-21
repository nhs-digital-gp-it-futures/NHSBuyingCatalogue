using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class ContactsDatastore : DatastoreBase<Contacts>, IContactsDatastore
  {
    public ContactsDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<ContactsDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public Contacts ById(string id)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.Get<Contacts>(id);
      });
    }

    public IQueryable<Contacts> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Contacts>().Where(c => c.OrganisationId == organisationId).AsQueryable();
      });
    }

    public Contacts ByEmail(string email)
    {
      return GetInternal(() =>
      {
        return _dbConnection.Value.GetAll<Contacts>().SingleOrDefault(c => c.EmailAddress1.ToLowerInvariant() == email.ToLowerInvariant());
      });
    }

    public Contacts Create(Contacts contact)
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

    public void Update(Contacts contact)
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

    public void Delete(Contacts contact)
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
