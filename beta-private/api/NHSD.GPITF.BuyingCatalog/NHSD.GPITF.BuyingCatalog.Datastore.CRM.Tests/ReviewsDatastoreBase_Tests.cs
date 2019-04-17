using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.Tests
{
  [TestFixture]
  public sealed class ReviewsDatastoreBase_Tests
  {
    private Mock<ILogger<DummyReviewsDatastoreBase>> _logger;
    private Mock<ISyncPolicyFactory> _policy;

    [SetUp]
    public void SetUp()
    {
      _logger = new Mock<ILogger<DummyReviewsDatastoreBase>>();
      _policy = new Mock<ISyncPolicyFactory>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyReviewsDatastoreBase(_logger.Object, _policy.Object));
    }

    [Test]
    public void Class_Implements_Interface()
    {
      var obj = new DummyReviewsDatastoreBase(_logger.Object, _policy.Object);

      var implInt = obj as IReviewsDatastore<ReviewsBase>;

      implInt.Should().NotBeNull();
    }
  }
}
