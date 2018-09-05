using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class CapabilitiesImplementedValidator_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ICapabilitiesImplementedDatastore> _claimDatastore;
    private Mock<ISolutionsDatastore> _solutionsDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _claimDatastore = new Mock<ICapabilitiesImplementedDatastore>();
      _solutionsDatastore = new Mock<ISolutionsDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object));
    }

    [TestCase(Roles.Supplier)]
    public void Validate_Delete_ValidRole_Draft_Succeeds(string role)
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = GetCapabilitiesImplemented(status: CapabilitiesImplementedStatus.Draft);
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim, ruleSet: nameof(ICapabilitiesImplementedLogic.Delete));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(Roles.Buyer)]
    [TestCase(Roles.Admin)]
    public void Validate_Delete_InvalidRole_Draft_ReturnsError(string role)
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = GetCapabilitiesImplemented(status: CapabilitiesImplementedStatus.Draft);
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim, ruleSet: nameof(ICapabilitiesImplementedLogic.Delete));

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Only supplier can delete a draft claim")
        .And
        .HaveCount(1);
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
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = GetCapabilitiesImplemented(status: status);
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim, ruleSet: nameof(ICapabilitiesImplementedLogic.Delete));

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Only supplier can delete a draft claim")
        .And
        .HaveCount(1);
    }

    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Draft, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Submitted, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Remediation, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Submitted, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Approved, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Rejected, Roles.Admin)]
    public void Validate_Update_ValidStatusTransition_Succeeds(CapabilitiesImplementedStatus oldStatus, CapabilitiesImplementedStatus newStatus, string role)
    {
      var orgId = Guid.NewGuid().ToString();
      var claimId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var soln = Creator.GetSolution(orgId: orgId);
      var oldClaim = GetCapabilitiesImplemented(id: claimId, status: oldStatus, solnId: soln.Id);
      var newClaim = GetCapabilitiesImplemented(id: claimId, status: newStatus, solnId: soln.Id);
      _claimDatastore.Setup(x => x.ById(claimId)).Returns(oldClaim);
      _solutionsDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      var valres = validator.Validate(newClaim, ruleSet: nameof(ICapabilitiesImplementedLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Draft, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Draft, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Submitted, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Submitted, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Remediation, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Remediation, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Remediation, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Approved, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Approved, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Approved, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Rejected, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Rejected, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Draft, CapabilitiesImplementedStatus.Rejected, Roles.Supplier)]

    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Remediation, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Remediation, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Draft, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Draft, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Draft, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Submitted, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Submitted, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Submitted, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Approved, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Approved, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Rejected, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Submitted, CapabilitiesImplementedStatus.Rejected, Roles.Supplier)]

    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Submitted, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Submitted, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Draft, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Draft, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Draft, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Remediation, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Remediation, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Remediation, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Approved, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Approved, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Approved, Roles.Supplier)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Rejected, Roles.Admin)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Rejected, Roles.Buyer)]
    [TestCase(CapabilitiesImplementedStatus.Remediation, CapabilitiesImplementedStatus.Rejected, Roles.Supplier)]
    public void Validate_Update_InvalidStatusTransition_ReturnsError(CapabilitiesImplementedStatus oldStatus, CapabilitiesImplementedStatus newStatus, string role)
    {
      var orgId = Guid.NewGuid().ToString();
      var claimId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var soln = Creator.GetSolution(orgId: orgId);
      var oldClaim = GetCapabilitiesImplemented(id: claimId, status: oldStatus, solnId: soln.Id);
      var newClaim = GetCapabilitiesImplemented(id: claimId, status: newStatus, solnId: soln.Id);
      _claimDatastore.Setup(x => x.ById(claimId)).Returns(oldClaim);
      _solutionsDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      var valres = validator.Validate(newClaim, ruleSet: nameof(ICapabilitiesImplementedLogic.Update));

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid Status transition")
        .And
        .HaveCount(1);
    }

    [Test]
    public void Validate_Update_FinalState_ReturnsError(
      [Values(
        CapabilitiesImplementedStatus.Approved,
        CapabilitiesImplementedStatus.Rejected)]
          CapabilitiesImplementedStatus oldStatus,
      [Values(
        CapabilitiesImplementedStatus.Draft,
        CapabilitiesImplementedStatus.Submitted,
        CapabilitiesImplementedStatus.Remediation,
        CapabilitiesImplementedStatus.Approved,
        CapabilitiesImplementedStatus.Rejected)]
          CapabilitiesImplementedStatus newStatus,
      [Values(
        Roles.Admin,
        Roles.Buyer,
        Roles.Supplier)]
          string role)
    {
      var orgId = Guid.NewGuid().ToString();
      var claimId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new CapabilitiesImplementedValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var soln = Creator.GetSolution(orgId: orgId);
      var oldClaim = GetCapabilitiesImplemented(id: claimId, status: oldStatus, solnId: soln.Id);
      var newClaim = GetCapabilitiesImplemented(id: claimId, status: newStatus, solnId: soln.Id);
      _claimDatastore.Setup(x => x.ById(claimId)).Returns(oldClaim);
      _solutionsDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      var valres = validator.Validate(newClaim, ruleSet: nameof(ICapabilitiesImplementedLogic.Update));

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid Status transition")
        .And
        .HaveCount(1);
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
