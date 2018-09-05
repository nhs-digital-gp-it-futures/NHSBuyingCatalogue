using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class LinkManagerValidator_Tests
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
      Assert.DoesNotThrow(() => new LinkManagerValidator(_context.Object));
    }

    [Test]
    public void Validate_Admin_Returns_NoError()
    {
      var ctx = Creator.GetContext(role: Roles.Admin);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new LinkManagerValidator(_context.Object);

      var valres = validator.Validate(_context.Object);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Buyer_Returns_Error()
    {
      var ctx = Creator.GetContext(role: Roles.Buyer);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new LinkManagerValidator(_context.Object);

      var valres = validator.Validate(_context.Object);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Must be admin")
        .And
        .HaveCount(1);
    }

    [Test]
    public void Validate_Supplier_Returns_Error()
    {
      var ctx = Creator.GetContext(role: Roles.Supplier);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new LinkManagerValidator(_context.Object);

      var valres = validator.Validate(_context.Object);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Must be admin")
        .And
        .HaveCount(1);
    }
  }
}
