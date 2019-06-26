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
  public sealed class ContactsDatastore : LongTermCachedDatastore<Contacts>, IContactsDatastore
  {
    private readonly GifInt.IContactsDatastore _crmDatastore;

    public ContactsDatastore(
      GifInt.IContactsDatastore crmDatastore,
      ILogger<ContactsDatastore> logger,
      ISyncPolicyFactory policy,
      IConfiguration config,
      ILongTermCache cache) :
      base(logger, policy, config, cache)
    {
      _crmDatastore = crmDatastore;
    }

    public Contacts ByEmail(string email)
    {
      return GetInternal(() =>
      {
        return GetFromCache(GetCachePathByEmail(email), email);
      });
    }

    public Contacts ById(string id)
    {
      return GetInternal(() =>
      {
        var val = _crmDatastore
          .ById(id);

        return Converter.FromCrm(val);
      });
    }

    public IEnumerable<Contacts> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .ByOrganisation(organisationId)
          .Select(val => Converter.FromCrm(val));

        return vals;
      });
    }

    protected override IEnumerable<Contacts> GetAllFromSource(string path, string parameter = null)
    {
      throw new NotImplementedException();
    }

    protected override Contacts GetFromSource(string path, string parameter)
    {
      if (path == GetCachePathByEmail(parameter))
      {
        var val = _crmDatastore
          .ByEmail(parameter);

        return Converter.FromCrm(val);
      }

      throw new ArgumentOutOfRangeException($"{nameof(path)}", path, "Unsupported cache path");
    }

    private static string GetCachePathByEmail(string email)
    {
      return $"/{nameof(Contacts)}/{nameof(ByEmail)}/{email}";
    }
  }
}
