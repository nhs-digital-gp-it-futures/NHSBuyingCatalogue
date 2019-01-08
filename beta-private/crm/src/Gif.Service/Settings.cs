using Microsoft.Extensions.Configuration;
using System;

namespace Gif.Service
{
  internal static class Settings
  {
    public static string GIF_AUTHORITY_URI(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_AUTHORITY_URI") ?? config["GIF:Authority_Uri"] ?? "http://localhost:5001";
    public static string GIF_CRM_AUTHORITY(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_CRM_AUTHORITY") ?? config["CrmAuthority"];
    public static string GIF_CRM_URL(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_CRM_URL") ?? config["CrmUrl"];
    public static string GIF_AZURE_CLIENT_ID(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_AZURE_CLIENT_ID") ?? config["AzureClientId"];
    public static string GIF_ENCRYPTED_CLIENT_SECRET(IConfiguration config) => Environment.GetEnvironmentVariable("GIF_ENCRYPTED_CLIENT_SECRET") ?? config["EncryptedClientSecret"];
  }
}
