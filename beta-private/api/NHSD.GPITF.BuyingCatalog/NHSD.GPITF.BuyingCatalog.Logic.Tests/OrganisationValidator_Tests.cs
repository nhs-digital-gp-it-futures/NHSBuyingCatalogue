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
  public sealed class OrganisationValidator_Tests
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
      Assert.DoesNotThrow(() => new OrganisationValidator(_context.Object));
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Supplier)]
    public void Validate_NonAdmin_Create_Returns_Error(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new OrganisationValidator(_context.Object);

      var valres = validator.Validate(new Organisation(), ruleSet: nameof(IOrganisationLogic.Create));

      valres.Errors.Count().Should().Be(1);
    }

    [TestCase(Roles.Admin)]
    public void Validate_Admin_Create_Completes(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new OrganisationValidator(_context.Object);

      var valres = validator.Validate(new Organisation(), ruleSet: nameof(IOrganisationLogic.Create));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Supplier)]
    public void Validate_NonAdmin_Update_Returns_Error(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new OrganisationValidator(_context.Object);

      var valres = validator.Validate(new Organisation(), ruleSet: nameof(IOrganisationLogic.Update));

      valres.Errors.Count().Should().Be(1);
    }

    [TestCase(Roles.Admin)]
    public void Validate_Admin_Update_Completes(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new OrganisationValidator(_context.Object);

      var valres = validator.Validate(new Organisation(), ruleSet: nameof(IOrganisationLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Supplier)]
    public void Validate_NonAdmin_Delete_Returns_Error(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new OrganisationValidator(_context.Object);

      var valres = validator.Validate(new Organisation(), ruleSet: nameof(IOrganisationLogic.Delete));

      valres.Errors.Count().Should().Be(1);
    }

    [TestCase(Roles.Admin)]
    public void Validate_Admin_Delete_Completes(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new OrganisationValidator(_context.Object);

      var valres = validator.Validate(new Organisation(), ruleSet: nameof(IOrganisationLogic.Delete));

      valres.Errors.Should().BeEmpty();
    }  }
}
