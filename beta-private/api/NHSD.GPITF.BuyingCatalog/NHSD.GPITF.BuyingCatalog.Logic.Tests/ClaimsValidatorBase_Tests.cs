using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class ClaimsValidatorBase_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<IClaimsDatastore<ClaimsBase>> _claimDatastore;
    private Mock<ISolutionsDatastore> _solutionsDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _claimDatastore = new Mock<IClaimsDatastore<ClaimsBase>>();
      _solutionsDatastore = new Mock<ISolutionsDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object));
    }

    [Test]
    public void Validate_Valid_Succeeds()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier, orgId: orgId));
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = Creator.GetClaimsBase();
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_IdNull_ReturnsError()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier, orgId: orgId));
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = Creator.GetClaimsBase();
      claim.Id = null;
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim);

      valres.Errors.Count().Should().Be(2);
    }

    [Test]
    public void Validate_IdNotGuid_ReturnsError()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier, orgId: orgId));
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = Creator.GetClaimsBase(id: "some other Id");
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim);

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_SolutionIdNull_ReturnsError()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier, orgId: orgId));
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = Creator.GetClaimsBase();
      claim.SolutionId = null;
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim);

      valres.Errors.Count().Should().Be(2);
    }

    [Test]
    public void Validate_SolutionIdNotGuid_ReturnsError()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier, orgId: orgId));
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = Creator.GetClaimsBase(solnId: "some other Id");
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim);

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Update_DifferentSolution_ReturnsError()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier, orgId: orgId));
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = Creator.GetClaimsBase();
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(Creator.GetClaimsBase());
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution());

      var valres = validator.Validate(claim, ruleSet: nameof(IClaimsLogic<ClaimsBase>.Update));

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Delete_OtherOrganisation_ReturnsError()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier, orgId: orgId));
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = Creator.GetClaimsBase();
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution());

      var valres = validator.Validate(claim, ruleSet: nameof(IClaimsLogic<ClaimsBase>.Delete));

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Delete_OwnOrganisation_Succeeds()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier, orgId: orgId));
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = Creator.GetClaimsBase();
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim, ruleSet: nameof(IClaimsLogic<ClaimsBase>.Delete));

      valres.Errors.Should().BeEmpty();
    }
  }
}
