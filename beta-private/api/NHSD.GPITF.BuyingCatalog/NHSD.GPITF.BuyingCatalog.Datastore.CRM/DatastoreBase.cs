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
using System.Globalization;
using System.Runtime.CompilerServices;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public abstract class DatastoreBase<T>
  {
    protected readonly IRestClientFactory _crmFactory;
    protected readonly ILogger<DatastoreBase<T>> _logger;
    private readonly ISyncPolicy _policy;

    public DatastoreBase(
      IRestClientFactory crmFactory,
      ILogger<DatastoreBase<T>> logger,
      ISyncPolicyFactory policy)
    {
      _crmFactory = crmFactory;
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

    protected string GetLogMessage(Organisations organisation, [CallerMemberName] string caller = "")
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
      request.AddHeader("Authorization", "Bearer " + _crmFactory.GetAccessToken()?.access_token);

      return request; 
    }

    protected RestRequest GetAllRequest(string path)
    {
      var request = GetRequest(path);
      AddGetAllParameters(request);

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

    protected RestRequest GetAllPostRequest(string path, object body)
    {
      var request = GetPostRequest(path, body);
      AddGetAllParameters(request);

      return request;
    }

    protected RestRequest GetPutRequest(string path, object body)
    {
      var request = GetRequest(path, body);
      request.Method = Method.PUT;

      return request;
    }

    protected RestRequest GetDeleteRequest(string path, object body)
    {
      var request = GetRequest(path, body);
      request.Method = Method.DELETE;

      return request;
    }

    protected IRestResponse GetRawResponse(RestRequest request)
    {
      return _crmFactory.GetRestClient().Execute(request);
    }

    protected TOther GetResponse<TOther>(RestRequest request)
    {
      var resp = GetRawResponse(request);
      var retval = JsonConvert.DeserializeObject<TOther>(resp.Content);

      return retval;
    }

    private static void AddGetAllParameters(RestRequest request)
    {
      const int StartPageIndex = 1;
      const int GetAllPageSize = 2000;

      request.AddQueryParameter("PageIndex", StartPageIndex.ToString(CultureInfo.InvariantCulture));
      request.AddQueryParameter("PageSize", GetAllPageSize.ToString(CultureInfo.InvariantCulture));
    }
  }
}
