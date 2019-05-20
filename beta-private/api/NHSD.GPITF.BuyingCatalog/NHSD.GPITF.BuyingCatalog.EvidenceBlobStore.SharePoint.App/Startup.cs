using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Owin.Logging;
using Owin;
using System;
using System.Reflection;
using System.Web.Http;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint.App
{
  public sealed class Startup
  {
    private IConfigurationRoot Config { get; }

    public Startup()
    {
      Config = Utils.GetConfiguration();

      // database connection string for nLog
      GlobalDiagnosticsContext.Set("LOG_CONNECTIONSTRING", Settings.LOG_CONNECTIONSTRING(Config));

      DumpSettings();
    }

    // This code configures Web API. The Startup class is specified as a type
    // parameter in the WebApp.Start method.
    public void Configuration(IAppBuilder app)
    {
      // Configure Web API for self-host.
      var config = new HttpConfiguration();

      // Web API routes
      config.MapHttpAttributeRoutes();

      config.Routes.MapHttpRoute(
          name: "EvidenceBlobStore",
          routeTemplate: "api/{controller}/{action}");

      app.UseWebApi(config);

      var builder = new ContainerBuilder();

      // Register Web API controller in executing assembly.
      builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

      builder
        .Register(cc => Config)
        .As<IConfiguration>();
      builder
        .RegisterGeneric(typeof(LoggerManager<>))
        .As(typeof(ILoggerManager<>))
        .InstancePerDependency();

      var container = builder.Build();

      config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

      // This will add the Autofac middleware as well as the middleware registered in the container.
      app.UseAutofacMiddleware(container);

      app.UseNLog();
    }

    private void DumpSettings()
    {
      Console.WriteLine("Settings:");
      Console.WriteLine($"  LOG:");
      Console.WriteLine($"    LOG_CONNECTIONSTRING : {Settings.LOG_CONNECTIONSTRING(Config)}");
      Console.WriteLine($"    LOG_SHAREPOINT       : {Settings.LOG_SHAREPOINT(Config)}");

      Console.WriteLine($"  SHAREPOINT:");
      Console.WriteLine($"    SHAREPOINT_BASEURL                  : {Settings.SHAREPOINT_BASEURL(Config)}");
      Console.WriteLine($"    SHAREPOINT_CLIENT_ID                : {Settings.SHAREPOINT_CLIENT_ID(Config)}");
      Console.WriteLine($"    SHAREPOINT_CLIENT_SECRET            : {Settings.SHAREPOINT_CLIENT_SECRET(Config)}");
      Console.WriteLine($"    SHAREPOINT_FILE_DOWNLOAD_BASE_URL   : {Settings.SHAREPOINT_FILE_DOWNLOAD_BASE_URL(Config)}");
    }
  }
}