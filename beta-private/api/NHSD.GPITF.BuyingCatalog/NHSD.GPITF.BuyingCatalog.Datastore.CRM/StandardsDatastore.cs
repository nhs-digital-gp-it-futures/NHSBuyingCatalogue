using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardsDatastore : DatastoreBase<Standards>, IStandardsDatastore
  {
    public StandardsDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<StandardsDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Standards> ByCapability(string capabilityId, bool isOptional)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"/Standard/ByCapability/{capabilityId}");
        request.AddQueryParameter("isOptional", isOptional.ToString().ToLowerInvariant());
        var retval = GetResponse<PaginatedList<Standards>>(request);

        return retval.Items.AsQueryable();
      });
    }

    public IQueryable<Standards> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"/Standard/ByFramework/{frameworkId}");
        var retval = GetResponse<PaginatedList<Standards>>(request);

        return retval.Items.AsQueryable();
      });
    }

    public Standards ById(string id)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"/Standard/ById/{id}");
        var retval = GetResponse<Standards>(request);

        return retval;
      });
    }

    public IQueryable<Standards> ByIds(IEnumerable<string> ids)
    {
      return GetInternal(() =>
      {
        var request = GetPostRequest("/Standard/ByIds", ids);
        request.AddQueryParameter("pageIndex", "1");
        request.AddQueryParameter("pageSize", int.MaxValue.ToString(CultureInfo.InvariantCulture));
        var retval = GetResponse<PaginatedList<Standards>>(request);

        return retval.Items.AsQueryable();
      });
    }

    public Standards Create(Standards standard)
    {
      return GetInternal(() =>
      {
        var request = GetPostRequest("/Standard/Create", standard);
        var retval = GetResponse<Standards>(request);

        return retval;
      });
    }

    public IQueryable<Standards> GetAll()
    {
      return GetInternal(() =>
      {
        var request = GetRequest("/Standard");
        request.AddQueryParameter("pageIndex", "1");
        request.AddQueryParameter("pageSize", int.MaxValue.ToString(CultureInfo.InvariantCulture));
        var retval = GetResponse<PaginatedList<Standards>>(request);

        return retval.Items.AsQueryable();
      });
    }

    public void Update(Standards standard)
    {
      GetInternal(() =>
      {
        var request = GetPutRequest("/Standard/Update", standard);
        var resp = GetRawResponse(request);

        return 0;
      });
    }
  }
}
