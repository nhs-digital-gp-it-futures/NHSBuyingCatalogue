using Microsoft.Extensions.Configuration;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App
{
  internal static class Utils
  {
    public static IConfigurationRoot GetConfiguration()
    {
      var builder = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile("hosting.json", optional: true, reloadOnChange: true)
        .AddUserSecrets<Program>();

      return builder.Build();
    }
  }
}
