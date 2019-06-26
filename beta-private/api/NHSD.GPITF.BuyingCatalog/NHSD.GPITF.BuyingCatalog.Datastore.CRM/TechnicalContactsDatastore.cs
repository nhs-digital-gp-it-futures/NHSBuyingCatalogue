using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Linq;
using GifInt = Gif.Service.Contracts;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class TechnicalContactsDatastore : CrmDatastoreBase<TechnicalContacts>, ITechnicalContactsDatastore
  {
    private readonly GifInt.ITechnicalContactsDatastore _crmDatastore;

    public TechnicalContactsDatastore(
      GifInt.ITechnicalContactsDatastore crmDatastore,
      ILogger<TechnicalContactsDatastore> logger,
      ISyncPolicyFactory policy) :
      base(logger, policy)
    {
      _crmDatastore = crmDatastore;
    }

    public IEnumerable<TechnicalContacts> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var vals = _crmDatastore
          .BySolution(solutionId)
          .Select(val => Converter.FromCrm(val));

        return vals;
      });
    }

    public TechnicalContacts Create(TechnicalContacts techCont)
    {
      return GetInternal(() =>
      {
        techCont.Id = UpdateId(techCont.Id);

        var val = _crmDatastore.Create(Converter.FromApi(techCont));

        return Converter.FromCrm(val);
      });
    }

    public void Delete(TechnicalContacts techCont)
    {
      GetInternal(() =>
      {
        _crmDatastore.Delete(Converter.FromApi(techCont));

        return 0;
      });
    }

    public void Update(TechnicalContacts techCont)
    {
      GetInternal(() =>
      {
        _crmDatastore.Update(Converter.FromApi(techCont));

        return 0;
      });
    }
  }
}
