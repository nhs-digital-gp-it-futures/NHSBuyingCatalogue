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
        StandardsApplicableStatus.PartiallyApproved,
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
