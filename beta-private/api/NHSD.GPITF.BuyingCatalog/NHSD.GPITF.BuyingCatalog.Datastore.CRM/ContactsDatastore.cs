using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

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

    private string ResourceBase { get; } = "/Contacts";

    public Contacts ByEmail(string email)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ByEmail/{email}");
        var retval = GetResponse<Contacts>(request);

        return retval;
      });
    }

    public Contacts ById(string id)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ById/{id}");
        var retval = GetResponse<Contacts>(request);

        return retval;
      });
    }

    public IEnumerable<Contacts> ByOrganisation(string organisationId)
    {
      return GetInternal(() =>
      {
        var request = GetAllRequest($"{ResourceBase}/ByOrganisation/{organisationId}");
        var retval = GetResponse<PaginatedList<Contacts>>(request);

        return retval.Items;
      });
    }
  }
}
