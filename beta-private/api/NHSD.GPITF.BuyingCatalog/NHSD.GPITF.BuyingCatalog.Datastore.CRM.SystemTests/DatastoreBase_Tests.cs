using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  public abstract class DatastoreBase_Tests<T>
  {
    protected IRestClientFactory _crmConnectionFactory;
    protected ILogger<T> _logger;
    protected ISyncPolicyFactory _policy;

    [SetUp]
    public void SetUp()
    {
      var builder = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile("hosting.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables()
          .AddUserSecrets<Program>();
      var config = builder.Build();

      _crmConnectionFactory = new RestClientFactory(config);
      _logger = new Mock<ILogger<T>>().Object;
      _policy = new RetryOnceSyncPolicyFactory();
    }
  }
}
