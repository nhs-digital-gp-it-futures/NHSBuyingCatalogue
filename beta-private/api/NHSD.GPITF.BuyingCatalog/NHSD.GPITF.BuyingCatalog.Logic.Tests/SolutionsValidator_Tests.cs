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
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(GetSolution(orgId: soln.OrganisationId));

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Update_DifferentOrganisation_ReturnsError()
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(GetSolution());

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Count().Should().Be(1);
    }

    [TestCase(SolutionStatus.Draft, SolutionStatus.Draft)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.Registered)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Failed)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Failed)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Approved)]
    public void Validate_Update_ValidStatusTransition_ReturnsNoError(SolutionStatus oldStatus, SolutionStatus newStatus)
    {
      var validator = new SolutionsValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var orgId = Guid.NewGuid().ToString();
      var oldSoln = GetSolution(status: oldStatus, orgId: orgId);
      var newSoln = GetSolution(status: newStatus, orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(oldSoln);

      var valres = validator.Validate(newSoln, ruleSet: nameof(ISolutionsLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(SolutionStatus.Draft, SolutionStatus.Failed)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.Draft, SolutionStatus.Approved)]

    [TestCase(SolutionStatus.Registered, SolutionStatus.Failed)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.Draft)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.Registered)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.Registered, SolutionStatus.Approved)]

    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Draft)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Registered)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.CapabilitiesAssessment, SolutionStatus.Approved)]

    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Draft)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Registered)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.StandardsCompliance, SolutionStatus.Approved)]

    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Failed)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Draft)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Registered)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.FinalApproval, SolutionStatus.Approved)]

    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Failed)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Draft)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.Registered)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.SolutionPage, SolutionStatus.FinalApproval)]

    [TestCase(SolutionStatus.Approved, SolutionStatus.Failed)]
    [TestCase(SolutionStatus.Approved, SolutionStatus.Draft)]
    [TestCase(SolutionStatus.Approved, SolutionStatus.Registered)]
    [TestCase(SolutionStatus.Approved, SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.Approved, SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.Approved, SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.Approved, SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.Approved, SolutionStatus.Approved)]

    [TestCase(SolutionStatus.Failed, SolutionStatus.Failed)]
    [TestCase(SolutionStatus.Failed, SolutionStatus.Draft)]
    [TestCase(SolutionStatus.Failed, SolutionStatus.Registered)]
    [TestCase(SolutionStatus.Failed, SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.Failed, SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.Failed, SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.Failed, SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.Failed, SolutionStatus.Approved)]
    public void Validate_Update_InvalidStatusTransition_ReturnsError(SolutionStatus oldStatus, SolutionStatus newStatus)
    {
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
