using Microsoft.Extensions.Configuration;
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

    private IConfigurationRoot Configuration { get; }

    public Program(string[] args)
    {
      Configuration = new ConfigurationBuilder()
        .AddJsonFile("hosting.json")
        .AddUserSecrets<Program>()
        .Build();

      DumpSettings();
      Console.WriteLine();
    }

    private void Run()
    {
      var dbConnFact = new DbConnectionFactory(Configuration);
      using (var conn = dbConnFact.Get())
      {
        Console.WriteLine($"Importing from:  {Settings.GIF_CRM_URL(Configuration)}");
        Console.WriteLine($"          into:  {conn.ConnectionString}");
      }
    }

    private void DumpSettings()
    {
      Console.WriteLine("Settings:");
      Console.WriteLine($"  CRM:");
      Console.WriteLine($"    CRM_CLIENTID            : {Settings.CRM_CLIENTID(Configuration)}");
      Console.WriteLine($"    CRM_CLIENTSECRET        : {Settings.CRM_CLIENTSECRET(Configuration)}");

      Console.WriteLine($"  GIF:");
      Console.WriteLine($"    GIF_CRM_URL                 : {Settings.GIF_CRM_URL(Configuration)}");
      Console.WriteLine($"    GIF_AUTHORITY_URI           : {Settings.GIF_AUTHORITY_URI(Configuration)}");
      Console.WriteLine($"    GIF_AZURE_CLIENT_ID         : {Settings.GIF_AZURE_CLIENT_ID(Configuration)}");
      Console.WriteLine($"    GIF_ENCRYPTED_CLIENT_SECRET : {Settings.GIF_ENCRYPTED_CLIENT_SECRET(Configuration)}");

      Console.WriteLine($"  DATASTORE:");
      Console.WriteLine($"    DATASTORE_CONNECTION        : {Settings.DATASTORE_CONNECTION(Configuration)}");
      Console.WriteLine($"    DATASTORE_CONNECTIONTYPE    : {Settings.DATASTORE_CONNECTIONTYPE(Configuration, Settings.DATASTORE_CONNECTION(Configuration))}");
      Console.WriteLine($"    DATASTORE_CONNECTIONSTRING  : {Settings.DATASTORE_CONNECTIONSTRING(Configuration, Settings.DATASTORE_CONNECTION(Configuration))}");
    }
  }
}
