using Microsoft.Owin.Hosting;
using System;
using System.Threading;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App
{
  public sealed class Program
  {
    private readonly AutoResetEvent _event = new AutoResetEvent(false);

    public void Start()
    {
      try
      {
        var config = Utils.GetConfiguration();
        var baseAddress = Settings.SHAREPOINT_FILE_DOWNLOAD_BASE_URL(config);

        // Start OWIN host
        using (WebApp.Start<Startup>(url: baseAddress))
        {
          // route CTRL-C through stop code for console operation
          Console.CancelKeyPress += (s, e) => Stop();

          Console.WriteLine("Press CTRL-C to exit");
          _event.WaitOne(Timeout.Infinite);
        }
      }
      finally
      {
        // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
        NLog.LogManager.Shutdown();
      }
    }

    public void Stop()
    {
      _event.Set();
    }

    static void Main()
    {
      new Program().Start();
    }
  }
}

