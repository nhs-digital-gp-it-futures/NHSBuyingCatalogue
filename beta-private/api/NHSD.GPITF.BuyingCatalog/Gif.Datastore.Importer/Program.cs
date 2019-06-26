using Gif.Service.Crm;
using Gif.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database;
using System;

namespace Gif.Datastore.Importer
{
  public sealed class Program
  {
    static void Main(string[] args)
    {
      new Program(args).Run();
    }

    private readonly IConfigurationRoot _config;
    private readonly IRepository _repo;

    public Program(string[] args)
    {
      var serviceProvider = new ServiceCollection()
        .AddSingleton<ILoggerFactory, LoggerFactory>()
        .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
        .BuildServiceProvider();
      var logger = serviceProvider.GetRequiredService<ILogger<IRepository>>();

      _config = new ConfigurationBuilder()
        .AddJsonFile("hosting.json")
        .AddUserSecrets<Program>()
        .Build();

      // output config as repo uses config
      DumpSettings();
      Console.WriteLine();

      _repo = new Repository(_config, logger);
    }

    private void Run()
    {
      var dbConnFact = new DbConnectionFactory(_config);
      using (var conn = dbConnFact.Get())
      {
        Console.WriteLine($"Importing");
        Console.WriteLine($"  from:  {Settings.GIF_CRM_URL(_config)}");
        Console.WriteLine($"  into:  {conn.ConnectionString}");

        // NHSD data
        // TODO   Frameworks
        var frameworksSvc = new FrameworksService(_repo);
        var frameworks = frameworksSvc.GetAll();

        // TODO   Capabilities
        var capabilitiesSvc = new CapabilitiesService(_repo);
        var capabilities = capabilitiesSvc.GetAll();

        // TODO   Standards
        var standardsSvc = new StandardsService(_repo);
        var standards = standardsSvc.GetAll();

        // TODO   CapabilityFramework
        // TODO   FrameworkStandard
        // TODO   CapabilityStandard

        // supplier data
        // TODO   Organisations
        // TODO   Contacts
        // TODO   Solutions
        // TODO   FrameworkSolution
        // TODO   ClaimedCapability
        // TODO   ClaimedStandard
        // TODO   ClaimedCapabilityEvidence
        // TODO   ClaimedStandardEvidence
        // TODO   ClaimedCapabilityReview
        // TODO   ClaimedStandardReview
        // TODO   TechnicalContact
        using (var trans = conn.BeginTransaction())
        {
          trans.Commit();
        }
        Console.WriteLine("Finished!");
      }
    }

    private void DumpSettings()
    {
      Console.WriteLine("Settings:");
      Console.WriteLine($"  CRM:");
      Console.WriteLine($"    CRM_CLIENTID            : {Settings.CRM_CLIENTID(_config)}");
      Console.WriteLine($"    CRM_CLIENTSECRET        : {Settings.CRM_CLIENTSECRET(_config)}");

      Console.WriteLine($"  GIF:");
      Console.WriteLine($"    GIF_CRM_URL                 : {Settings.GIF_CRM_URL(_config)}");
      Console.WriteLine($"    GIF_AUTHORITY_URI           : {Settings.GIF_AUTHORITY_URI(_config)}");
      Console.WriteLine($"    GIF_AZURE_CLIENT_ID         : {Settings.GIF_AZURE_CLIENT_ID(_config)}");
      Console.WriteLine($"    GIF_ENCRYPTED_CLIENT_SECRET : {Settings.GIF_ENCRYPTED_CLIENT_SECRET(_config)}");

      Console.WriteLine($"  DATASTORE:");
      Console.WriteLine($"    DATASTORE_CONNECTION        : {Settings.DATASTORE_CONNECTION(_config)}");
      Console.WriteLine($"    DATASTORE_CONNECTIONTYPE    : {Settings.DATASTORE_CONNECTIONTYPE(_config, Settings.DATASTORE_CONNECTION(_config))}");
      Console.WriteLine($"    DATASTORE_CONNECTIONSTRING  : {Settings.DATASTORE_CONNECTIONSTRING(_config, Settings.DATASTORE_CONNECTION(_config))}");
    }
  }
}
