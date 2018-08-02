using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class ClaimedCapabilityValidator_Tests
  {
    private Mock<IHttpContextAccessor> _context;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new ClaimedCapabilityValidator(_context.Object));
    }
  }
}
