using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Tests
{
  [TestFixture]
  public sealed class EvidenceDatastoreBase_Tests
  {
    private Mock<IRestClientFactory> _crmConnectionFactory;
    private Mock<ILogger<EvidenceDatastoreBase<EvidenceBase>>> _logger;
    private Mock<ISyncPolicyFactory> _policy;

    [SetUp]
    public void SetUp()
    {
      _crmConnectionFactory = new Mock<IRestClientFactory>();
      _logger = new Mock<ILogger<EvidenceDatastoreBase<EvidenceBase>>>();
      _policy = new Mock<ISyncPolicyFactory>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyEvidenceDatastoreBase(_crmConnectionFactory.Object, _logger.Object, _policy.Object));
    }

    [Test]
    public void Class_Implements_Interface()
    {
      var obj = new DummyEvidenceDatastoreBase(_crmConnectionFactory.Object, _logger.Object, _policy.Object);

      var implInt = obj as IEvidenceDatastore<EvidenceBase>;

      implInt.Should().NotBeNull();
    }
  }
}
