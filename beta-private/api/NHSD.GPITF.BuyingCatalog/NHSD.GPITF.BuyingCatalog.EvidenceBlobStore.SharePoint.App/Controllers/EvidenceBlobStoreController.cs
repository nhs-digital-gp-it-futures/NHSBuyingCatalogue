using Microsoft.Extensions.Configuration;
using Microsoft.SharePoint.Client;
using NetFramework.Console;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App.Controllers
{
  /// <summary>
  /// Manage capabilities+standards evidence
  /// </summary>
  public sealed class EvidenceBlobStoreController : ApiController
  {
    private readonly bool _logSharePoint;
    private readonly ILoggerManager<EvidenceBlobStoreController> _logger;

    private readonly string SHAREPOINT_BASEURL;
    private readonly string SHAREPOINT_CLIENT_ID;
    private readonly string SHAREPOINT_CLIENT_SECRET;

    public EvidenceBlobStoreController(IConfiguration config, ILoggerManager<EvidenceBlobStoreController> logger)
    {
      _logSharePoint = Settings.LOG_SHAREPOINT(config);
      _logger = logger;

      SHAREPOINT_BASEURL = Settings.SHAREPOINT_BASEURL(config);
      SHAREPOINT_CLIENT_ID = Settings.SHAREPOINT_CLIENT_ID(config);
      SHAREPOINT_CLIENT_SECRET = Settings.SHAREPOINT_CLIENT_SECRET(config);
    }

    public EvidenceBlobStoreController()
    {
    }

    /// <summary>
    /// Download a file which is supporting a claim
    /// </summary>
    /// <param name="serverRelativeUrl">URL of file relative to server</param>
    /// <returns>FileResult with suggested file download name based on extUrl</returns>
    [HttpPost]
    public HttpResponseMessage Download([FromBody]string serverRelativeUrl)
    {
      var ctx = ClientContextByApp();
      var stream = DownloadFile_BinaryStream(ctx, serverRelativeUrl);
      var result = new HttpResponseMessage(HttpStatusCode.OK);
      result.Content = new StreamContent(stream);
      result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = Path.GetFileName(serverRelativeUrl)
      };
      result.Content.Headers.ContentLength = stream.Length;

      return result;
    }

    private ClientContext ClientContextByApp()
    {
      var ctx = new CSOM.AuthenticationManager(new ConsoleLogger()).GetAppOnlyAuthenticatedContext(SHAREPOINT_BASEURL, SHAREPOINT_CLIENT_ID, SHAREPOINT_CLIENT_SECRET);
      ctx.ApplicationName = "GP IT Futures - Evidence Store";

      return ctx;
    }

    private void LogInfo(string msg)
    {
      if (_logSharePoint)
      {
        _logger.LogInfo(msg);
      }
    }

    private Stream DownloadFile_BinaryStream(ClientContext ctx, string serverRelativeUrl)
    {
      LogInfo($"{nameof(DownloadFile_BinaryStream)} --> {serverRelativeUrl}");

      var file = ctx.Web.GetFileByServerRelativeUrl(serverRelativeUrl);
      var data = file.OpenBinaryStream();
      ctx.Load(file);
      ctx.ExecuteQuery();

      return data.Value;
    }
  }
}
