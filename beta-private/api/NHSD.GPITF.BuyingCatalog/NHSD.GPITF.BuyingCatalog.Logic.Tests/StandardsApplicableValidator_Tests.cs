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
  public sealed class StandardsApplicableValidator_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<IStandardsApplicableDatastore> _claimDatastore;
    private Mock<ISolutionsDatastore> _solutionsDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _claimDatastore = new Mock<IStandardsApplicableDatastore>();
      _solutionsDatastore = new Mock<ISolutionsDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new StandardsApplicableValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object));
    }

    [Test]
    public void Validate_Delete_ValidStatusRole_Draft_Succeeds(
      [Values(
        Roles.Supplier
      )]
        string role,
      [Values(
        StandardsApplicableStatus.NotStarted,
        StandardsApplicableStatus.Draft
      )]
        StandardsApplicableStatus status)
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new StandardsApplicableValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = GetStandardsApplicable(status: status);
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim, ruleSet: nameof(IStandardsApplicableLogic.Delete));

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Delete_InvalidStatusRole_Draft_Succeeds(
      [Values(
        Roles.Buyer,
        Roles.Admin
      )]
        string role,
      [Values(
        StandardsApplicableStatus.Submitted,
        StandardsApplicableStatus.Remediation,
        StandardsApplicableStatus.Approved,
        StandardsApplicableStatus.ApprovedFirstOfType,
        StandardsApplicableStatus.ApprovedPartial,
        StandardsApplicableStatus.Rejected
      )]
        StandardsApplicableStatus status)
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new StandardsApplicableValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var claim = GetStandardsApplicable(status: status);
      _claimDatastore.Setup(x => x.ById(claim.Id)).Returns(claim);
      _solutionsDatastore.Setup(x => x.ById(claim.SolutionId)).Returns(Creator.GetSolution(orgId: orgId));

      var valres = validator.Validate(claim, ruleSet: nameof(IStandardsApplicableLogic.Delete));

      valres.Errors.Should()
        .ContainSingle()
        .And
        .ContainSingle(x => x.ErrorMessage == "Only supplier can delete a draft claim");
    }

    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Draft, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Submitted, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.Submitted, StandardsApplicableStatus.Remediation, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Remediation, StandardsApplicableStatus.Submitted, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.Submitted, StandardsApplicableStatus.Rejected, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Submitted, StandardsApplicableStatus.Approved, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Submitted, StandardsApplicableStatus.ApprovedFirstOfType, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Submitted, StandardsApplicableStatus.ApprovedPartial, Roles.Admin)]
    public void Validate_Update_ValidStatusTransition_Succeeds(StandardsApplicableStatus oldStatus, StandardsApplicableStatus newStatus, string role)
    {
      var orgId = Guid.NewGuid().ToString();
      var claimId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new StandardsApplicableValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var soln = Creator.GetSolution(orgId: orgId);
      var oldClaim = GetStandardsApplicable(id: claimId, status: oldStatus, solnId: soln.Id);
      var newClaim = GetStandardsApplicable(id: claimId, status: newStatus, solnId: soln.Id);
      _claimDatastore.Setup(x => x.ById(claimId)).Returns(oldClaim);
      _solutionsDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      var valres = validator.Validate(newClaim, ruleSet: nameof(IStandardsApplicableLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Draft, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Draft, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.NotStarted, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.NotStarted, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.NotStarted, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Submitted, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Submitted, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Submitted, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Remediation, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Remediation, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Remediation, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Approved, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Approved, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Approved, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.ApprovedFirstOfType, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.ApprovedFirstOfType, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.ApprovedFirstOfType, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.ApprovedPartial, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.ApprovedPartial, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.ApprovedPartial, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Rejected, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Rejected, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.NotStarted, StandardsApplicableStatus.Rejected, Roles.Supplier)]

    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Submitted, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Submitted, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.NotStarted, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.NotStarted, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.NotStarted, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Remediation, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Remediation, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Remediation, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Approved, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Approved, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Approved, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.ApprovedFirstOfType, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.ApprovedFirstOfType, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.ApprovedFirstOfType, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.ApprovedPartial, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.ApprovedPartial, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.ApprovedPartial, Roles.Supplier)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Rejected, Roles.Admin)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Rejected, Roles.Buyer)]
    [TestCase(StandardsApplicableStatus.Draft, StandardsApplicableStatus.Rejected, Roles.Supplier)]
    public void Validate_Update_InvalidStatusTransition_ReturnsError(StandardsApplicableStatus oldStatus, StandardsApplicableStatus newStatus, string role)
    {
      var orgId = Guid.NewGuid().ToString();
      var claimId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new StandardsApplicableValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var soln = Creator.GetSolution(orgId: orgId);
      var oldClaim = GetStandardsApplicable(id: claimId, status: oldStatus, solnId: soln.Id);
      var newClaim = GetStandardsApplicable(id: claimId, status: newStatus, solnId: soln.Id);
      _claimDatastore.Setup(x => x.ById(claimId)).Returns(oldClaim);
      _solutionsDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      var valres = validator.Validate(newClaim, ruleSet: nameof(IStandardsApplicableLogic.Update));

      valres.Errors.Should()
        .ContainSingle()
        .And
        .ContainSingle(x => x.ErrorMessage == "Invalid Status transition");
    }

    [Test]
    public void Validate_Update_FinalState_ReturnsError(
      [Values(
        StandardsApplicableStatus.Approved,
        StandardsApplicableStatus.ApprovedPartial,
        StandardsApplicableStatus.ApprovedFirstOfType,
        StandardsApplicableStatus.Rejected)]
          StandardsApplicableStatus oldStatus,
      [Values(
        StandardsApplicableStatus.NotStarted,
        StandardsApplicableStatus.Draft,
        StandardsApplicableStatus.Submitted,
        StandardsApplicableStatus.Remediation,
        StandardsApplicableStatus.Approved,
        StandardsApplicableStatus.ApprovedPartial,
        StandardsApplicableStatus.ApprovedFirstOfType,
        StandardsApplicableStatus.Rejected)]
          StandardsApplicableStatus newStatus,
      [Values(
        Roles.Admin,
        Roles.Buyer,
        Roles.Supplier)]
          string role)
    {
      var orgId = Guid.NewGuid().ToString();
      var claimId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role, orgId: orgId));
      var validator = new StandardsApplicableValidator(_context.Object, _claimDatastore.Object, _solutionsDatastore.Object);
      var soln = Creator.GetSolution(orgId: orgId);
      var oldClaim = GetStandardsApplicable(id: claimId, status: oldStatus, solnId: soln.Id);
      var newClaim = GetStandardsApplicable(id: claimId, status: newStatus, solnId: soln.Id);
      _claimDatastore.Setup(x => x.ById(claimId)).Returns(oldClaim);
      _solutionsDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      var valres = validator.Validate(newClaim, ruleSet: nameof(IStandardsApplicableLogic.Update));

      valres.Errors.Should()
        .ContainSingle()
        .And
        .ContainSingle(x => x.ErrorMessage == "Invalid Status transition");
    }

    private static StandardsApplicable GetStandardsApplicable(
      string id = null,
      string solnId = null,
      string claimId = null,
      StandardsApplicableStatus status = StandardsApplicableStatus.Draft)
    {
      return new StandardsApplicable
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString(),
        StandardId = claimId ?? Guid.NewGuid().ToString(),
        Status = status
      };
    }
  }
}
