using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class TechnicalContactsDatastore : DatastoreBase<TechnicalContacts>, ITechnicalContactsDatastore
  {
    public TechnicalContactsDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<TechnicalContactsDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<TechnicalContacts> BySolution(string solutionId)
    {
      throw new NotImplementedException();
    }

    public TechnicalContacts Create(TechnicalContacts techCont)
    {
      throw new NotImplementedException();
    }

    public void Delete(TechnicalContacts techCont)
    {
      throw new NotImplementedException();
    }

    public void Update(TechnicalContacts techCont)
    {
      throw new NotImplementedException();
    }
  }
}
