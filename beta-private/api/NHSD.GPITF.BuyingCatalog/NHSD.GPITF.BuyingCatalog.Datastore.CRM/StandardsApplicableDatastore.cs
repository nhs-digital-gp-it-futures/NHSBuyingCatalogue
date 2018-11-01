﻿using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class StandardsApplicableDatastore : DatastoreBase<StandardsApplicable>, IStandardsApplicableDatastore
  {
    public StandardsApplicableDatastore(
      IRestClientFactory crmConnectionFactory,
      ILogger<StandardsApplicableDatastore> logger,
      ISyncPolicyFactory policy) :
      base(crmConnectionFactory, logger, policy)
    {
    }

    private string ResourceBase { get; } = "/StandardsApplicable";

    public StandardsApplicable ById(string id)
    {
      return GetInternal(() =>
      {
        var request = GetRequest($"{ResourceBase}/ById/{id}");
        var retval = GetResponse<StandardsApplicable>(request);

        return retval;
      });
    }

    public IEnumerable<StandardsApplicable> BySolution(string solutionId)
    {
      return GetInternal(() =>
      {
        var request = GetAllRequest($"{ResourceBase}/BySolution/{solutionId}");
        var retval = GetResponse<PaginatedList<StandardsApplicable>>(request);

        return retval.Items;
      });
    }

    public StandardsApplicable Create(StandardsApplicable claimedstandard)
    {
      return GetInternal(() =>
      {
        var request = GetPutRequest($"{ResourceBase}", claimedstandard);
        var retval = GetResponse<StandardsApplicable>(request);

        return retval;
      });
    }

    public void Delete(StandardsApplicable claimedstandard)
    {
      GetInternal(() =>
      {
        var request = GetDeleteRequest($"{ResourceBase}", claimedstandard);
        var resp = GetRawResponse(request);

        return 0;
      });
    }

    public void Update(StandardsApplicable claimedstandard)
    {
      GetInternal(() =>
      {
        var request = GetPostRequest($"{ResourceBase}", claimedstandard);
        var resp = GetRawResponse(request);

        return 0;
      });
    }
  }
}
