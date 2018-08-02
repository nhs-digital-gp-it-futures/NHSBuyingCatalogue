using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardDatastore : DatastoreBase<Standard>, IStandardDatastore
  {
    public StandardDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<StandardDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    public IQueryable<Standard> ByCapability(string capabilityId, bool isOptional)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"/Standard/ByCapability/{capabilityId}");
        request.AddQueryParameter("isOptional", isOptional.ToString().ToLowerInvariant());
        var retval = GetResponse<PaginatedList<Standard>>(request);

        return retval.Items.AsQueryable();
      });
    }

    public IQueryable<Standard> ByFramework(string frameworkId)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"/Standard/ByFramework/{frameworkId}");
        var retval = GetResponse<PaginatedList<Standard>>(request);

        return retval.Items.AsQueryable();
      });
    }

    public Standard ById(string id)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"/Standard/ById/{id}");
        var retval = GetResponse<Standard>(request);

        return retval;
      });
    }

    public IQueryable<Standard> ByIds(IEnumerable<string> ids)
    {
      return GetInternal(() =>
      {
        var request = GetPostRequest("/Standard/ByIds", ids);
        request.AddQueryParameter("pageIndex", "1");
        request.AddQueryParameter("pageSize", int.MaxValue.ToString(CultureInfo.InvariantCulture));
        var retval = GetResponse<PaginatedList<Standard>>(request);

        return retval.Items.AsQueryable();
      });
    }

    public Standard Create(Standard standard)
    {
      return GetInternal(() =>
      {
        var request = GetPostRequest("/Standard/Create", standard);
        var retval = GetResponse<Standard>(request);

        return retval;
      });
    }

    public IQueryable<Standard> GetAll()
    {
      return GetInternal(() =>
      {
        var request = GetRequest("/Standard");
        request.AddQueryParameter("pageIndex", "1");
        request.AddQueryParameter("pageSize", int.MaxValue.ToString(CultureInfo.InvariantCulture));
        var retval = GetResponse<PaginatedList<Standard>>(request);

        return retval.Items.AsQueryable();
      });
    }

    public void Update(Standard standard)
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
