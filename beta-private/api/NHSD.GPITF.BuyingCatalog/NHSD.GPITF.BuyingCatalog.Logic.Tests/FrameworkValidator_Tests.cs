using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class FrameworkValidator_Tests
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
      Assert.DoesNotThrow(() => new FrameworkValidator(_context.Object));
    }

    [Test]
    public void Validate_Admin_Create_Returns_NoError()
    {
      var ctx = Creator.GetContext(role: Roles.Admin);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new FrameworkValidator(_context.Object);

      var valres = validator.Validate(new Framework(), ruleSet: nameof(IFrameworkLogic.Create));

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Admin_Update_Returns_NoError()
    {
      var ctx = Creator.GetContext(role: Roles.Admin);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new FrameworkValidator(_context.Object);

      var valres = validator.Validate(new Framework(), ruleSet: nameof(IFrameworkLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Supplier)]
    public void Validate_NonAdmin_Create_Returns_Error(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new FrameworkValidator(_context.Object);

      var valres = validator.Validate(new Framework(), ruleSet: nameof(IFrameworkLogic.Create));

      valres.Errors.Count().Should().Be(1);
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Supplier)]
    public void Validate_NonAdmin_Update_Returns_Error(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new FrameworkValidator(_context.Object);

      var valres = validator.Validate(new Framework(), ruleSet: nameof(IFrameworkLogic.Update));

      valres.Errors.Count().Should().Be(1);
    }
  }
}
