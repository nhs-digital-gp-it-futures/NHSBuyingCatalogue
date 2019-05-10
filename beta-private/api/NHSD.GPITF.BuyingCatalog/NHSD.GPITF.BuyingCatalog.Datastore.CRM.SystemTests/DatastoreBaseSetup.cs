using Gif.Service.Contracts;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [SetUpFixture]
  public sealed class DatastoreBaseSetup
  {
    public static ISolutionsDatastore SolutionsDatastore { get; internal set; }
    public static ITechnicalContactsDatastore TechnicalContactsDatastore { get; internal set; }
    public static ICapabilitiesImplementedDatastore CapabilitiesImplementedDatastore { get; internal set; }
    public static ICapabilitiesImplementedEvidenceDatastore CapabilitiesImplementedEvidenceDatastore { get; internal set; }
    public static ICapabilitiesImplementedReviewsDatastore CapabilitiesImplementedReviewsDatastore { get; internal set; }
    public static IStandardsApplicableDatastore StandardsApplicableDatastore { get; internal set; }
    public static IStandardsApplicableEvidenceDatastore StandardsApplicableEvidenceDatastore { get; internal set; }
    public static IStandardsApplicableReviewsDatastore StandardsApplicableReviewsDatastore { get; internal set; }
    public static ISolutionsExDatastore SolutionsExDatastore { get; internal set; }
    public static IOrganisationsDatastore OrganisationsDatastore { get; internal set; }
    public static IFrameworksDatastore FrameworksDatastore { get; internal set; }
    public static IContactsDatastore ContactsDatastore { get; internal set; }
    public static ICapabilityDatastore CapabilitiesDatastore { get; internal set; }
    public static IStandardsDatastore StandardsDatastore { get; internal set; }
    public static ICapabilityStandardDatastore CapabilityStandardDatastore { get; internal set; }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      var builder = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile("hosting.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables()
          .AddUserSecrets<Program>();
      var config = builder.Build();
    }
  }
}
