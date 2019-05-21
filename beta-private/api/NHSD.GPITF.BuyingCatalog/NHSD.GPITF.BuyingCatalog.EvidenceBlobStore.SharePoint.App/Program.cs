using Microsoft.Owin.Hosting;
using System;
using System.Threading;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App
{
  public sealed class Program
  {
    private Program()
    {
    }

    static void Main()
    {
      try
      {
        var config = Utils.GetConfiguration();
        var baseAddress = Settings.SHAREPOINT_FILE_DOWNLOAD_BASE_URL(config);

        // Start OWIN host
        using (WebApp.Start<Startup>(url: baseAddress))
        {
          Console.WriteLine("Press CTRL-C to exit");
          Thread.Sleep(Timeout.Infinite);
        }
      }
      finally
      {
        // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
        NLog.LogManager.Shutdown();
      }
    }
  }
}

