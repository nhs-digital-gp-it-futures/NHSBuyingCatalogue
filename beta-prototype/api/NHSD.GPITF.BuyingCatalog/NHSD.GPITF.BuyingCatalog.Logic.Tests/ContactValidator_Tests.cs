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
  public sealed class ContactValidator_Tests
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
      Assert.DoesNotThrow(() => new ContactValidator(_context.Object));
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Supplier)]
    public void Validate_NonAdmin_Create_Returns_Error(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new ContactValidator(_context.Object);

      var valres = validator.Validate(new Contact(), ruleSet: nameof(IContactLogic.Create));

      valres.Errors.Count().Should().Be(1);
    }

    [TestCase(Roles.Admin)]
    public void Validate_Admin_Create_Completes(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new ContactValidator(_context.Object);

      var valres = validator.Validate(new Contact(), ruleSet: nameof(IContactLogic.Create));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Supplier)]
    public void Validate_NonAdmin_Update_Returns_Error(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new ContactValidator(_context.Object);

      var valres = validator.Validate(new Contact(), ruleSet: nameof(IContactLogic.Update));

      valres.Errors.Count().Should().Be(1);
    }

    [TestCase(Roles.Admin)]
    public void Validate_Admin_Update_Completes(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new ContactValidator(_context.Object);

      var valres = validator.Validate(new Contact(), ruleSet: nameof(IContactLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Supplier)]
    public void Validate_NonAdmin_Delete_Returns_Error(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new ContactValidator(_context.Object);

      var valres = validator.Validate(new Contact(), ruleSet: nameof(IContactLogic.Delete));

      valres.Errors.Count().Should().Be(1);
    }

    [TestCase(Roles.Admin)]
    public void Validate_Admin_Delete_Completes(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new ContactValidator(_context.Object);

      var valres = validator.Validate(new Contact(), ruleSet: nameof(IContactLogic.Delete));

      valres.Errors.Should().BeEmpty();
    }
  }
}
