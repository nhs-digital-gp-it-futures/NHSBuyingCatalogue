using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using RestSharp;
using System;
using System.Configuration;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class RestClientFactory : IRestClientFactory
  {
    private readonly string ApiUri;
    private readonly string AccessTokenUri;

    private readonly string ClientId;
    private readonly string ClientSecret;

    public RestClientFactory(IConfiguration config)
    {
    // read out of user secret or environment
      ApiUri = config["CRM:ApiUri"] ?? Environment.GetEnvironmentVariable("CRM:ApiUri");
      AccessTokenUri = config["CRM:AccessTokenUri"] ?? Environment.GetEnvironmentVariable("CRM:AccessTokenUri");

      ClientId = config["CRM:ClientId"] ?? Environment.GetEnvironmentVariable("CRM:ClientId");
      ClientSecret = config["CRM:ClientSecret"] ?? Environment.GetEnvironmentVariable("CRM:ClientSecret");

      if (string.IsNullOrWhiteSpace(ApiUri) ||
        string.IsNullOrWhiteSpace(AccessTokenUri) ||
        string.IsNullOrWhiteSpace(ClientId) ||
        string.IsNullOrWhiteSpace(ClientSecret)
        )
      {
        throw new ConfigurationErrorsException("Missing CRM configuration - check UserSecrets or environment variables");
      }
    }

    public IRestClient GetRestClient()
    {
      return new RestClient(ApiUri);
    }

    public AccessToken GetAccessToken()
    {
      var restclient = new RestClient(AccessTokenUri);
      var request = new RestRequest() { Method = Method.POST };
      request.AddHeader("Accept", "application/json");
      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      request.AddParameter("client_id", ClientId);
      request.AddParameter("client_secret", ClientSecret);
      request.AddParameter("grant_type", "client_credentials");
      var resp = restclient.Execute(request);
      var responseJson = resp.Content;
      var token = JsonConvert.DeserializeObject<AccessToken>(responseJson);

      return token.access_token.Length > 0 ? token : null;
    }
  }
}
