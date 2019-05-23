using System;
using Topshelf;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.Service
{
  public sealed class Program
  {
    private Program()
    {
    }

    static void Main(string[] args)
    {
      // based on Topshelf, so supports:
      //    -install
      //    -uninstall
      // commandline switches:
      //    http://docs.topshelf-project.com/en/latest/overview/commandline.html
      // which obviously have to be run with admin rights
      var rc = HostFactory.Run(x =>
      {
        x.Service<SharePoint.App.Program>(s =>
        {
          s.ConstructUsing(name => new SharePoint.App.Program());
          s.WhenStarted(tc => tc.Start());
          s.WhenStopped(tc => tc.Stop());
        });
        x.RunAsLocalSystem();
        x.StartAutomatically();

        x.EnableServiceRecovery(r =>
        {
          r.RestartService(TimeSpan.FromMinutes(1));
        });

        x.SetDescription("Download evidence files from SharePoint");
        x.SetDisplayName("NHSD Evidence Blob Store Service");
        x.SetServiceName("NHSDEvidenceBlobStoreService");
      });

      var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
      Environment.ExitCode = exitCode;
    }
  }
}
