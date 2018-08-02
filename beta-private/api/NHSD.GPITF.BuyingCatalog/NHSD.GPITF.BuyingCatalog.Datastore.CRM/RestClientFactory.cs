using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Configuration;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM
{
  public sealed class RestClientFactory : IRestClientFactory
  {
    private readonly string AuthorityUri;
    private readonly string ServiceUri;

    private readonly string ResourceUri;
    private readonly string ClientId;
    private readonly string Secret;

    public RestClientFactory(IConfiguration config)
    {
    // read out of user secret or environment
      AuthorityUri = config["CRM:AuthorityUri"] ?? Environment.GetEnvironmentVariable("CRM:AuthorityUri");
      ServiceUri = config["CRM:ServiceUri"] ?? Environment.GetEnvironmentVariable("CRM:ServiceUri");

      ResourceUri = config["CRM:ResourceUri"] ?? Environment.GetEnvironmentVariable("CRM_:esourceUri");
      ClientId = config["CRM:ClientId"] ?? Environment.GetEnvironmentVariable("CRM:ClientId");
      Secret = config["CRM:Secret"] ?? Environment.GetEnvironmentVariable("CRM:Secret");

      if (string.IsNullOrWhiteSpace(AuthorityUri) ||
        string.IsNullOrWhiteSpace(ServiceUri) ||
        string.IsNullOrWhiteSpace(ResourceUri) ||
        string.IsNullOrWhiteSpace(ClientId) ||
        string.IsNullOrWhiteSpace(Secret)
        )
      {
        throw new ConfigurationErrorsException("Missing CRM configuration - check UserSecrets or environment variables");
      }
    }

    public IRestClient Get()
    {
      var clientCredential = new ClientCredential(ClientId, Secret);
      var authContext = new AuthenticationContext(AuthorityUri);
      var accessToken = authContext.AcquireTokenAsync(ResourceUri, clientCredential).Result;
      var authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken.CreateAuthorizationHeader());
      var client = new RestClient(ServiceUri)
      {
        Authenticator = authenticator
      };

      return client;
    }
  }
}
