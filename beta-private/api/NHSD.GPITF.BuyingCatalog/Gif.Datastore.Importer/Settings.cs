using Microsoft.Extensions.Configuration;
using System;

namespace Gif.Datastore.Importer
{
  public static class Settings
  {
    public static string CRM_CLIENTID(IConfiguration config) => Environment.GetEnvironmentVariable("CRM_CLIENTID") ?? config["CRM:ClientId"];
    public static string CRM_CLIENTSECRET(IConfiguration config) => Environment.GetEnvironmentVariable("CRM_CLIENTSECRET") ?? config["CRM:ClientSecret"];

    public static string GIF_CRM_URL(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_CRM_URL") ?? config["CrmUrl"];
    public static string GIF_AUTHORITY_URI(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_AUTHORITY_URI") ?? config["GIF:Authority_Uri"] ?? "http://localhost:5001";
    public static string GIF_AZURE_CLIENT_ID(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_AZURE_CLIENT_ID") ?? config["AzureClientId"];
    public static string GIF_ENCRYPTED_CLIENT_SECRET(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_ENCRYPTED_CLIENT_SECRET") ?? config["EncryptedClientSecret"];

    public static string DATASTORE_CONNECTION(IConfiguration config) => Environment.GetEnvironmentVariable("DATASTORE_CONNECTION") ?? config["RepositoryDatabase:Connection"];
    public static string DATASTORE_CONNECTIONTYPE(IConfiguration config, string connection) => Environment.GetEnvironmentVariable("DATASTORE_CONNECTIONTYPE") ?? config[$"RepositoryDatabase:{connection}:Type"];
    public static string DATASTORE_CONNECTIONSTRING(IConfiguration config, string connection) => (Environment.GetEnvironmentVariable("DATASTORE_CONNECTIONSTRING") ?? config[$"RepositoryDatabase:{connection}:ConnectionString"]);
  }
}
