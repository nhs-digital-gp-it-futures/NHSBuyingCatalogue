using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using Polly;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class DatastoreBase<T>
  {
    protected readonly IRestClient _crmConnection;
    protected readonly ILogger<DatastoreBase<T>> _logger;
    private readonly ISyncPolicy _policy;

    public DatastoreBase(
      IRestClientFactory crmConnectionFactory,
      ILogger<DatastoreBase<T>> logger,
      ISyncPolicyFactory policy)
    {
      _crmConnection = crmConnectionFactory.Get();
      _logger = logger;
      _policy = policy.Build(_logger);
    }

    protected TOther GetInternal<TOther>(Func<TOther> get)
    {
      return _policy.Execute(get);
    }

    protected string GetLogMessage(IEnumerable<T> infos, [CallerMemberName] string caller = "")
    {
      return caller + " --> " + JArray.FromObject(infos).ToString(Formatting.None);
    }

    protected string GetLogMessage(Organisation organisation, [CallerMemberName] string caller = "")
    {
      return caller + " --> " + JObject.FromObject(organisation).ToString(Formatting.None);
    }

    protected string GetLogMessage(object info, [CallerMemberName] string caller = "")
    {
      return caller + " --> " + JObject.FromObject(info).ToString(Formatting.None);
    }

    protected RestRequest GetRequest(string path)
    {
      var request = new RestRequest(path)
      {
        Method = Method.GET
      };
      request.AddHeader("Content-Type", "application/json");

      return request;
    }

    protected RestRequest GetRequest(string path, object body)
    {
      var request = GetRequest(path);
      request.AddJsonBody(body);

      return request;
    }

    protected RestRequest GetPostRequest(string path, object body)
    {
      var request = GetRequest(path, body);
      request.Method = Method.POST;

      return request;
    }

    protected RestRequest GetPutRequest(string path, object body)
    {
      var request = GetRequest(path, body);
      request.Method = Method.PUT;

      return request;
    }

    protected IRestResponse GetRawResponse(RestRequest request)
    {
      return _crmConnection.Execute(request);
    }

    protected TOther GetResponse<TOther>(RestRequest request)
    {
      var resp = GetRawResponse(request);
      var retval = JsonConvert.DeserializeObject<TOther>(resp.Content);

      return retval;
    }
  }
}
