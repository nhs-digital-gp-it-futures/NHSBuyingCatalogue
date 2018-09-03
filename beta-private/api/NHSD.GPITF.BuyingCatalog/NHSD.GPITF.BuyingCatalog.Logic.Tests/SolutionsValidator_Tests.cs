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
  public sealed class SolutionsValidator_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsDatastore> _solutionDatastore;
    private Mock<IOrganisationsDatastore> _organisationDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
      _organisationDatastore = new Mock<IOrganisationsDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object));
    }

    [Test]
    public void Validate_Valid_ReturnsNoError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();

      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_IdNull_ReturnsError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      soln.Id = null;

      var valres = validator.Validate(soln);

      valres.Errors.Count().Should().Be(2);
    }

    [Test]
    public void Validate_IdNotGuid_ReturnsError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution(id: "some other Id");

      var valres = validator.Validate(soln);

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_OrganisationIdNull_ReturnsError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution(orgId: null);
      soln.OrganisationId = null;

      var valres = validator.Validate(soln);

      valres.Errors.Count().Should().Be(2);
    }

    [Test]
    public void Validate_OrganisationIdNotGuid_ReturnsError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution(orgId: "some other Id");

      var valres = validator.Validate(soln);

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Update_Valid_ReturnsNoError()
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(GetSolution(orgId: soln.OrganisationId));

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Update_DifferentOrganisation_ReturnsError()
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: Roles.Supplier));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(GetSolution());

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Count().Should().Be(1);
    }

    [TestCase(SolutionStatus.Draft, SolutionStatus.Draft, Roles.Supplier)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.Registered, Roles.Supplier)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.CapabilitiesAssessment, Roles.Supplier)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Failed, Roles.Admin)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.StandardsCompliance, Roles.Admin)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Failed, Roles.Admin)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.FinalApproval, Roles.Admin)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.SolutionPage, Roles.Admin)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Approved, Roles.Admin)]
    public void Validate_Update_ValidStatusTransition_ReturnsNoError(SolutionStatus oldStatus, SolutionStatus newStatus, string role)
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var orgId = Guid.NewGuid().ToString();
      var oldSoln = GetSolution(status: oldStatus, orgId: orgId);
      var newSoln = GetSolution(status: newStatus, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(SolutionStatus.Draft, SolutionStatus.Failed, Roles.Buyer)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.CapabilitiesAssessment, Roles.Buyer)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.StandardsCompliance, Roles.Buyer)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.SolutionPage, Roles.Buyer)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.FinalApproval, Roles.Buyer)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.Approved, Roles.Buyer)]

    [TestCase(SolutionStatus.Draft, SolutionStatus.Failed, Roles.Admin)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.CapabilitiesAssessment, Roles.Admin)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.StandardsCompliance, Roles.Admin)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.SolutionPage, Roles.Admin)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.FinalApproval, Roles.Admin)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.Approved, Roles.Admin)]

    [TestCase(SolutionStatus.Registered, SolutionStatus.Failed, Roles.Buyer)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.Draft, Roles.Buyer)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.Registered, Roles.Buyer)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.StandardsCompliance, Roles.Buyer)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.SolutionPage, Roles.Buyer)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.FinalApproval, Roles.Buyer)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.Approved, Roles.Buyer)]

    [TestCase(SolutionStatus.Registered, SolutionStatus.Failed, Roles.Admin)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.Draft, Roles.Admin)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.Registered, Roles.Admin)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.StandardsCompliance, Roles.Admin)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.SolutionPage, Roles.Admin)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.FinalApproval, Roles.Admin)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.Approved, Roles.Admin)]

    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Draft, Roles.Buyer)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Registered, Roles.Buyer)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.CapabilitiesAssessment, Roles.Buyer)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.SolutionPage, Roles.Buyer)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.FinalApproval, Roles.Buyer)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Approved, Roles.Buyer)]

    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Draft, Roles.Supplier)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Registered, Roles.Supplier)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.CapabilitiesAssessment, Roles.Supplier)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.SolutionPage, Roles.Supplier)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.FinalApproval, Roles.Supplier)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Approved, Roles.Supplier)]

    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Draft, Roles.Buyer)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Registered, Roles.Buyer)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.CapabilitiesAssessment, Roles.Buyer)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.StandardsCompliance, Roles.Buyer)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.SolutionPage, Roles.Buyer)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Approved, Roles.Buyer)]

    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Draft, Roles.Supplier)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Registered, Roles.Supplier)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.CapabilitiesAssessment, Roles.Supplier)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.StandardsCompliance, Roles.Supplier)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.SolutionPage, Roles.Supplier)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Approved, Roles.Supplier)]

    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Failed, Roles.Buyer)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Draft, Roles.Buyer)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Registered, Roles.Buyer)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.CapabilitiesAssessment, Roles.Buyer)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.StandardsCompliance, Roles.Buyer)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.FinalApproval, Roles.Buyer)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Approved, Roles.Buyer)]

    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Failed, Roles.Supplier)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Draft, Roles.Supplier)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Registered, Roles.Supplier)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.CapabilitiesAssessment, Roles.Supplier)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.StandardsCompliance, Roles.Supplier)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.FinalApproval, Roles.Supplier)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Approved, Roles.Supplier)]

    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Failed, Roles.Buyer)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Draft, Roles.Buyer)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Registered, Roles.Buyer)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.CapabilitiesAssessment, Roles.Buyer)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.StandardsCompliance, Roles.Buyer)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.SolutionPage, Roles.Buyer)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.FinalApproval, Roles.Buyer)]

    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Failed, Roles.Supplier)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Draft, Roles.Supplier)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Registered, Roles.Supplier)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.CapabilitiesAssessment, Roles.Supplier)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.StandardsCompliance, Roles.Supplier)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.SolutionPage, Roles.Supplier)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.FinalApproval, Roles.Supplier)]
    public void Validate_Update_InvalidStatusTransition_ReturnsError(SolutionStatus oldStatus, SolutionStatus newStatus, string role)
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var orgId = Guid.NewGuid().ToString();
      var oldSoln = GetSolution(status: oldStatus, orgId: orgId);
      var newSoln = GetSolution(status: newStatus, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Count.Should().Be(1);
    }

    [Test]
    public void Validate_Update_FinalState_ReturnsError(
      [Values(
        SolutionStatus.Approved,
        SolutionStatus.Failed)]
          SolutionStatus oldStatus,
      [Values(
        SolutionStatus.Failed,
        SolutionStatus.Draft,
        SolutionStatus.Registered,
        SolutionStatus.CapabilitiesAssessment,
        SolutionStatus.StandardsCompliance,
        SolutionStatus.SolutionPage,
        SolutionStatus.FinalApproval,
        SolutionStatus.Approved)]
          SolutionStatus newStatus,
      [Values(
        Roles.Admin,
        Roles.Buyer,
        Roles.Supplier)]
          string role)
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var orgId = Guid.NewGuid().ToString();
      var oldSoln = GetSolution(status: oldStatus, orgId: orgId);
      var newSoln = GetSolution(status: newStatus, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Count.Should().Be(1);
    }

    private static Solutions GetSolution(
      string id = null,
      string orgId = null,
      SolutionStatus status = SolutionStatus.Draft)
    {
      return new Solutions
      {
        Id = id ?? Guid.NewGuid().ToString(),
        OrganisationId = orgId ?? Guid.NewGuid().ToString(),
        Status = status
      };
    }
  }
}
