using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace NHSD.GPITF.BuyingCatalog
{
  internal static class WebHostBuilder
  {
    public static IWebHost BuildWebHost(string[] args)
    {
      var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("hosting.json")
        .Build();

      return WebHost.CreateDefaultBuilder(args)
        .UseConfiguration(config)
        .UseKestrel()
        .UseStartup<Startup>()
        .Build();
    }
  }
}
