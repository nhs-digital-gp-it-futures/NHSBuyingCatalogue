using Dapper.Contrib.Extensions;
using Gif.Service.Crm;
using Gif.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM;
using NHSD.GPITF.BuyingCatalog.Datastore.Database;
using System;
using System.Linq;

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

      Console.WriteLine($"Connecting to CRM...");
      _repo = new Repository(_config, logger);
      Console.WriteLine();
    }

    private void Run()
    {
      var dbConnFact = new DbConnectionFactory(_config);
      using (var conn = dbConnFact.Get())
      {
        Console.WriteLine($"Importing data:");
        Console.WriteLine($"  from:  {Settings.GIF_CRM_URL(_config)}");
        Console.WriteLine($"  into:  {conn.ConnectionString}");
        Console.WriteLine();

        Console.WriteLine($"Retrieving data from CRM:");

        #region NHSD data
        Console.WriteLine($"  Frameworks...");
        var frameworksSvc = new FrameworksService(_repo);
        var frameworks = frameworksSvc
          .GetAll().Select(x => Creator.FromCrm(x))
          .ToList();

        Console.WriteLine($"  Capabilities...");
        var capsSvc = new CapabilitiesService(_repo);
        var caps = capsSvc
          .GetAll().Select(x => Creator.FromCrm(x))
          .ToList();

        Console.WriteLine($"  Standards...");
        var stdsSvc = new StandardsService(_repo);
        var stds = stdsSvc
          .GetAll().Select(x => Creator.FromCrm(x))
          .ToList();

        Console.WriteLine($"  TODO   CapabilityFramework...");
        Console.WriteLine($"  TODO   FrameworkStandard...");
        Console.WriteLine($"  TODO   CapabilityStandard...");
        #endregion

        #region Supplier data
        Console.WriteLine($"  Organisations...");
        var orgsSvc = new OrganisationsService(_repo);
        var orgs = orgsSvc
          .GetAll().Select(x => Creator.FromCrm(x))
          .ToList();

        Console.WriteLine($"  Contacts...");
        var contactsSvc = new ContactsService(_repo);
        var contacts = orgs
          .SelectMany(org => contactsSvc.ByOrganisation(org.Id))
          .Select(cont => Creator.FromCrm(cont))
          .ToList();

        Console.WriteLine($"  Solutions...");
        var solnsSvc = new SolutionsService(_repo);
        var solns = orgs
          .SelectMany(org => solnsSvc.ByOrganisation(org.Id))
          .Select(cont => Creator.FromCrm(cont))
          .ToList();

        Console.WriteLine($"  TechnicalContact...");
        var techContSvc = new TechnicalContactService(_repo);
        var techConts = solns
          .SelectMany(soln => techContSvc.BySolution(soln.Id))
          .Select(techCont => Creator.FromCrm(techCont))
          .ToList();

        Console.WriteLine($"  TODO   FrameworkSolution...");

        Console.WriteLine($"  ClaimedCapability...");
        var claimedCapsSvc = new CapabilitiesImplementedService(_repo);
        var claimedCaps = solns
          .SelectMany(soln => claimedCapsSvc.BySolution(soln.Id))
          .Select(cont => Creator.FromCrm(cont))
          .ToList();

        Console.WriteLine($"  ClaimedStandard...");
        var claimedStdsSvc = new StandardsApplicableService(_repo);
        var claimedStds = solns
          .SelectMany(soln => claimedStdsSvc.BySolution(soln.Id))
          .Select(cont => Creator.FromCrm(cont))
          .ToList();

        Console.WriteLine($"  ClaimedCapabilityEvidence...");
        var claimedCapsEvSvc = new CapabilitiesImplementedEvidenceService(_repo);
        var claimedCapsEv = claimedCaps
          .SelectMany(claim => claimedCapsEvSvc.ByClaim(claim.Id).SelectMany(x => x))
          .Select(ev => Creator.FromCrm(ev))
          .ToList();

        Console.WriteLine($"  ClaimedStandardEvidence...");
        var claimedStdsEvSvc = new StandardsApplicableEvidenceService(_repo);
        var claimedStdsEv = claimedStds
          .SelectMany(claim => claimedStdsEvSvc.ByClaim(claim.Id).SelectMany(x => x))
          .Select(ev => Creator.FromCrm(ev))
          .ToList();

        Console.WriteLine($"  ClaimedCapabilityReview...");
        var claimedCapsRevSvc = new CapabilitiesImplementedReviewsService(_repo);
        var claimedCapsRev = claimedCapsEv
          .SelectMany(ev => claimedCapsRevSvc.ByEvidence(ev.Id).SelectMany(x => x))
          .Select(rev => Creator.CapabilitiesImplementedReviewsFromCrm(rev))
          .ToList();

        Console.WriteLine($"  ClaimedStandardReview...");
        var claimedStdsRevSvc = new StandardsApplicableReviewsService(_repo);
        var claimedStdsRev = claimedStdsEv
          .SelectMany(ev => claimedStdsRevSvc.ByEvidence(ev.Id).SelectMany(x => x))
          .Select(rev => Creator.StandardsApplicableReviewsFromCrm(rev))
          .ToList();
        #endregion

        using (var trans = conn.BeginTransaction())
        {
          // NHSD data
          conn.Insert(frameworks, trans);
          conn.Insert(caps, trans);
          conn.Insert(stds, trans);

          // supplier data
          conn.Insert(orgs, trans);
          conn.Insert(contacts, trans);
          conn.Insert(solns, trans);
          conn.Insert(techConts, trans);

          conn.Insert(claimedCaps, trans);
          conn.Insert(claimedStds, trans);

          conn.Insert(claimedCapsEv, trans);
          conn.Insert(claimedStdsEv, trans);

          conn.Insert(claimedCapsRev, trans);
          conn.Insert(claimedStdsRev, trans);

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
