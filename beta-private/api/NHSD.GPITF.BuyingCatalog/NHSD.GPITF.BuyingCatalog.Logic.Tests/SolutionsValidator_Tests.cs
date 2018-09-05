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
    public void Validate_Valid_Succeeds()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = Creator.GetSolution();

      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_IdNull_ReturnsError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = Creator.GetSolution();
      soln.Id = null;

      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "Invalid Id")
        .And
        .Contain(x => x.ErrorMessage == "'Id' must not be empty.")
        .And
        .HaveCount(2);
    }

    [Test]
    public void Validate_IdNotGuid_ReturnsError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = Creator.GetSolution(id: "some other Id");

      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid Id")
        .And
        .HaveCount(1);
    }

    [Test]
    public void Validate_OrganisationIdNull_ReturnsError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = Creator.GetSolution(orgId: null);
      soln.OrganisationId = null;

      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .Contain(x => x.ErrorMessage == "Invalid OrganisationId")
        .And
        .Contain(x => x.ErrorMessage == "'Organisation Id' must not be empty.")
        .And
        .HaveCount(2);
    }

    [Test]
    public void Validate_OrganisationIdNotGuid_ReturnsError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = Creator.GetSolution(orgId: "some other Id");

      var valres = validator.Validate(soln);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid OrganisationId")
        .And
        .HaveCount(1);
    }

    [Test]
    public void Validate_Update_Valid_Succeeds()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(orgId: orgId, role: Roles.Supplier));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = Creator.GetSolution(orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(Creator.GetSolution(orgId: soln.OrganisationId));

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Update_DifferentOrganisation_ReturnsError()
    {
      var orgId = Guid.NewGuid().ToString();
      var soln = Creator.GetSolution(orgId: orgId);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(orgId: orgId, role: Roles.Supplier));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(Creator.GetSolution());

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Cannot transfer solutions between organisations")
        .And
        .HaveCount(1);
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
    public void Validate_Update_ValidStatusTransition_Succeeds(SolutionStatus oldStatus, SolutionStatus newStatus, string role)
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(orgId: orgId, role: role));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var oldSoln = Creator.GetSolution(status: oldStatus, orgId: orgId);
      var newSoln = Creator.GetSolution(status: newStatus, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(newSoln.Id)).Returns(oldSoln);

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
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(orgId: orgId, role: role));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var oldSoln = Creator.GetSolution(status: oldStatus, orgId: orgId);
      var newSoln = Creator.GetSolution(status: newStatus, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(newSoln.Id)).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid Status transition")
        .And
        .HaveCount(1);
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
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(orgId: orgId, role: role));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var oldSoln = Creator.GetSolution(status: oldStatus, orgId: orgId);
      var newSoln = Creator.GetSolution(status: newStatus, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(newSoln.Id)).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid Status transition")
        .And
        .HaveCount(1);
    }

    [Test]
    public void Validate_Update_SupplierFromSameOrganisation_Succeeds()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(orgId: orgId, role: Roles.Supplier));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var oldSoln = Creator.GetSolution(status: SolutionStatus.Draft, orgId: orgId);
      var newSoln = Creator.GetSolution(status: SolutionStatus.Draft, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(newSoln.Id)).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Update_SupplierFromDifferentOrganisation_ReturnsError(
      [Values(
        Roles.Supplier)]
          string role)
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var oldSoln = Creator.GetSolution(status: SolutionStatus.Draft, orgId: orgId);
      var newSoln = Creator.GetSolution(status: SolutionStatus.Draft, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(newSoln.Id)).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Must be from same organisation")
        .And
        .HaveCount(1);
    }

    [Test]
    public void Validate_Update_NonSupplierFromDifferentOrganisation_ReturnsError(
      [Values(
        Roles.Admin,
        Roles.Buyer)]
          string role)
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var oldSoln = Creator.GetSolution(status: SolutionStatus.Draft, orgId: orgId);
      var newSoln = Creator.GetSolution(status: SolutionStatus.Draft, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(newSoln.Id)).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Must be from same organisation")
        .And
        .ContainSingle(x => x.ErrorMessage == "Invalid Status transition")
        .And
        .HaveCount(2);
    }

    [Test]
    public void Validate_Update_NoPreviousVersion_Succeeds()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(orgId: orgId, role: Roles.Supplier));
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var oldSoln = Creator.GetSolution(status: SolutionStatus.Draft, orgId: orgId);
      var newSoln = Creator.GetSolution(status: SolutionStatus.Draft, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(newSoln.Id)).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should().BeEmpty();
    }
  }
}
