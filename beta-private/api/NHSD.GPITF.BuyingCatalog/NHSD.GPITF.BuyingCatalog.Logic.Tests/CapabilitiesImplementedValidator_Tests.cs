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
  public sealed class CapabilitiesImplementedValidator_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ICapabilitiesImplementedDatastore> _claimDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _claimDatastore = new Mock<ICapabilitiesImplementedDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object));
    }

    [TestCase(Roles.Supplier)]
    public void Validate_Delete_ValidRole_Draft_ReturnsNoError(string role)
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object);
      var claim = GetCapabilitiesImplemented(status: CapabilitiesImplementedStatus.Draft);

      var valres = validator.Validate(claim, ruleSet: nameof(ICapabilitiesImplementedLogic.Delete));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Admin)]
    public void Validate_Delete_InvalidRole_Draft_ReturnsError(string role)
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object);
      var claim = GetCapabilitiesImplemented(status: CapabilitiesImplementedStatus.Draft);

      var valres = validator.Validate(claim, ruleSet: nameof(ICapabilitiesImplementedLogic.Delete));

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Delete_NonDraft_ReturnsError(
      [Values(
        Roles.Buyer,
        Roles.Supplier,
        Roles.Admin
      )]
        string role,
      [Values(
        CapabilitiesImplementedStatus.Submitted,
        CapabilitiesImplementedStatus.Remediation,
        CapabilitiesImplementedStatus.Approved,
        CapabilitiesImplementedStatus.Rejected
        )]
        CapabilitiesImplementedStatus status)
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object);
      var claim = GetCapabilitiesImplemented(status: status);

      var valres = validator.Validate(claim, ruleSet: nameof(ICapabilitiesImplementedLogic.Delete));

      valres.Errors.Count().Should().Be(1);
    }

    private static CapabilitiesImplemented GetCapabilitiesImplemented(
      string id = null,
      string solnId = null,
      string claimId = null,
      CapabilitiesImplementedStatus status = CapabilitiesImplementedStatus.Draft)
    {
      return new CapabilitiesImplemented
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString(),
        CapabilityId = claimId ?? Guid.NewGuid().ToString(),
        Status = status
      };
    }
  }
}
