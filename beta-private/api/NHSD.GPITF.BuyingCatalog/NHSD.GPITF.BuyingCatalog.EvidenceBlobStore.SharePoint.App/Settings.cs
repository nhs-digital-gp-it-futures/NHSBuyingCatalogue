using Microsoft.Extensions.Configuration;
using System;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App
{
  public static class Settings
  {
    public static string LOG_CONNECTIONSTRING(IConfiguration config) => Environment.GetEnvironmentVariable("LOG_CONNECTIONSTRING") ?? config["Log:ConnectionString"];
    public static bool LOG_SHAREPOINT(IConfiguration config) => bool.Parse(Environment.GetEnvironmentVariable("LOG_SHAREPOINT") ?? config["Log:SharePoint"] ?? false.ToString());

    public static string SHAREPOINT_BASEURL(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_BASEURL") ?? config["SharePoint:BaseUrl"];
    public static string SHAREPOINT_CLIENT_ID(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_CLIENT_ID") ?? config["SharePoint:ClientId"];
    public static string SHAREPOINT_CLIENT_SECRET(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_CLIENT_SECRET") ?? config["SharePoint:ClientSecret"];
    public static string SHAREPOINT_FILE_DOWNLOAD_BASE_URL(IConfiguration config) => Environment.GetEnvironmentVariable("SHAREPOINT_FILE_DOWNLOAD_BASE_URL") ?? config["SharePoint:FileDownloadBaseUrl"] ?? "http://localhost:9000/";
  }
}