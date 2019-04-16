using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class ContactsDatastore : CachedDatastore<Contacts>, IContactsDatastore
  {
    private readonly GifInt.IContactsDatastore _crmDatastore;

    public ContactsDatastore(
      GifInt.IContactsDatastore crmDatastore,
      ILogger<ContactsDatastore> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      IDatastoreCache cache) :
      base(logger, policy, config, cache)
    {
      _crmDatastore = crmDatastore;
    }

    public Contacts ByEmail(string email)
    {
      return GetInternal(() =>
      {
        return Get(GetCachePathByEmail(email), email);
      });
    }

    public Contacts ById(string id)
    {
      return GetInternal(() =>
      {
        var val = _crmDatastore
          .ById(id);

        return Creator.FromCrm(val);
      });
    }

    public IEnumerable<Contacts> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByOrganisation(organisationId)
          .Select(val => Creator.FromCrm(val));

        return vals;
      });
    }

    protected override IEnumerable<Contacts> GetAllInternal(string path)
    {
      throw new System.NotImplementedException();
    }

    protected override Contacts GetInternal(string path, string parameter)
    {
      if (path == GetCachePathByEmail(parameter))
      {
        var val = _crmDatastore
          .ByEmail(parameter);

        return Creator.FromCrm(val);
      }

      throw new ArgumentOutOfRangeException($"{nameof(path)}", path, "Unsupported path");
    }

    private static string GetCachePathByEmail(string email)
    {
      return $"/{nameof(Contacts)}/ByEmail/{email}";
    }
  }
}
