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
  public sealed class SolutionValidator_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionDatastore> _solutionDatastore;
    private Mock<IOrganisationDatastore> _organisationDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _solutionDatastore = new Mock<ISolutionDatastore>();
      _organisationDatastore = new Mock<IOrganisationDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object));
    }

    [Test]
    public void Validate_Valid_ReturnsNoError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();

      var valres = validator.Validate(soln);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_IdNull_ReturnsError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      soln.Id = null;

      var valres = validator.Validate(soln);

      valres.Errors.Count().Should().Be(2);
    }

    [Test]
    public void Validate_IdNotGuid_ReturnsError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution(id: "some other Id");

      var valres = validator.Validate(soln);

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_OrganisationIdNull_ReturnsError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution(orgId: null);
      soln.OrganisationId = null;

      var valres = validator.Validate(soln);

      valres.Errors.Count().Should().Be(2);
    }

    [Test]
    public void Validate_OrganisationIdNotGuid_ReturnsError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution(orgId: "some other Id");

      var valres = validator.Validate(soln);

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Update_Valid_ReturnsNoError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(GetSolution(orgId: soln.OrganisationId));

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionLogic.Update));

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Update_DifferentOrganisation_ReturnsError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(GetSolution());

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionLogic.Update));

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Delete_Valid_ReturnsNoError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(GetSolution());
      _organisationDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(new Organisation());

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionLogic.Delete));

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Delete_NoSolution_ReturnsError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      _organisationDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(new Organisation());

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionLogic.Delete));

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Delete_NoOrganisation_ReturnsError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();
      _solutionDatastore.Setup(x => x.ById(It.IsAny<string>())).Returns(GetSolution());

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionLogic.Delete));

      valres.Errors.Count().Should().Be(1);
    }

    [Test]
    public void Validate_Delete_NoSolutionNoOrganisation_ReturnsError()
    {
      var validator = new SolutionValidator(_context.Object, _solutionDatastore.Object, _organisationDatastore.Object);
      var soln = GetSolution();

      var valres = validator.Validate(soln, ruleSet: nameof(ISolutionLogic.Delete));

      valres.Errors.Count().Should().Be(2);
    }

    private static Solution GetSolution(
      string id = null,
      string orgId = null)
    {
      return new Solution
      {
        Id = id ?? Guid.NewGuid().ToString(),
        OrganisationId = orgId ?? Guid.NewGuid().ToString()
      };
    }
  }
}
