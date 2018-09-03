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

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _claimDatastore = new Mock<IClaimsDatastore<ClaimsBase>>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object));
    }

    [Test]
    public void Validate_Valid_ReturnsNoError()
    {
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object);
      var claim = GetClaimsBase();

      var valres = validator.Validate(claim);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_IdNull_ReturnsError()
    {
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object);
      var claim = GetClaimsBase();
      claim.Id = null;

      var valres = validator.Validate(claim);

      valres.Errors.Count().Should().Be(2);
    }

    [Test]
    public void Validate_IdNotGuid_ReturnsError()
    {
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object);
      var claim = GetClaimsBase(id: "some other Id");

      var valres = validator.Validate(claim);

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_SolutionIdNull_ReturnsError()
    {
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object);
      var claim = GetClaimsBase();
      claim.SolutionId = null;

      var valres = validator.Validate(claim);

      valres.Errors.Count().Should().Be(2);
    }

    [Test]
    public void Validate_SolutionIdNotGuid_ReturnsError()
    {
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object);
      var claim = GetClaimsBase(solnId: "some other Id");

      var valres = validator.Validate(claim);

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Update_DifferentSolution_ReturnsError()
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier));
      var validator = new DummyClaimsValidatorBase(_context.Object, _claimDatastore.Object);
      var claim = GetClaimsBase();
      _claimDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(GetClaimsBase());

      var valres = validator.Validate(claim, ruleSet: nameof(IClaimsLogic<ClaimsBase>.Update));

      valres.Errors.Count().Should().Be(1);
    }

    private static ClaimsBase GetClaimsBase(
      string id = null,
      string solnId = null)
    {
      return new DummyClaimsBase
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString()
      };
    }
  }
}
