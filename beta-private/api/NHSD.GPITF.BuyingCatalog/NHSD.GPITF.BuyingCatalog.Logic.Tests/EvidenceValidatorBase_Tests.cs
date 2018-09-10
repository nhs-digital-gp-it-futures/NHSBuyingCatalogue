using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class EvidenceValidatorBase_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<IEvidenceDatastore<EvidenceBase>> _evidenceDatastore;
    private Mock<IStandardsApplicableDatastore> _claimDatastore;
    private Mock<ISolutionsDatastore> _solutionDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _evidenceDatastore = new Mock<IEvidenceDatastore<EvidenceBase>>();
      _claimDatastore = new Mock<IStandardsApplicableDatastore>();
      _claimDatastore.As<IClaimsDatastore<ClaimsBase>>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object));
    }

    [Test]
    public void MustBeValidClaimId_Valid_Succeeds()
    {
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase();

      validator.MustBeValidClaimId();
      var valres = validator.Validate(evidence);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void MustBeValidClaimId_Null_ReturnsError()
    {
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase();
      evidence.ClaimId = null;

      validator.MustBeValidClaimId();
      var valres = validator.Validate(evidence);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid ClaimId")
        .And
        .ContainSingle(x => x.ErrorMessage == "'Claim Id' must not be empty.")
        .And
        .HaveCount(2);
    }

    [Test]
    public void MustBeValidClaimId_NotGuid_ReturnsError()
    {
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase(claimId: "some other Id");

      validator.MustBeValidClaimId();
      var valres = validator.Validate(evidence);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid ClaimId")
        .And
        .HaveCount(1);
    }

    [TestCase(Roles.Supplier)]
    public void MustBeSupplier_Supplier_Succeeds(string role)
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase();

      validator.MustBeSupplier();
      var valres = validator.Validate(evidence);

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(Roles.Admin)]
    [TestCase(Roles.Buyer)]
    public void MustBeSupplier_NonSupplier_ReturnsError(string role)
    {
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(role: role));
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase();

      validator.MustBeSupplier();
      var valres = validator.Validate(evidence);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Must be supplier")
        .And
        .HaveCount(1);
    }

    [Test]
    public void MustBeFromSameOrganisation_Same_Succeeds()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext(orgId: orgId));
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var soln = Creator.GetSolution(orgId: orgId);
      var claim = Creator.GetClaimsBase(solnId: soln.Id);
      var evidence = Creator.GetEvidenceBase();
      _claimDatastore.As<IClaimsDatastore<ClaimsBase>>().Setup(x => x.ById(evidence.ClaimId)).Returns(claim);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      validator.MustBeFromSameOrganisation();
      var valres = validator.Validate(evidence);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void MustBeFromSameOrganisation_Other_ReturnsError()
    {
      var orgId = Guid.NewGuid().ToString();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var soln = Creator.GetSolution(orgId: orgId);
      var claim = Creator.GetClaimsBase(solnId: soln.Id);
      var evidence = Creator.GetEvidenceBase();
      _claimDatastore.As<IClaimsDatastore<ClaimsBase>>().Setup(x => x.ById(evidence.ClaimId)).Returns(claim);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      validator.MustBeFromSameOrganisation();
      var valres = validator.Validate(evidence);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Must be from same organisation")
        .And
        .HaveCount(1);
    }

    [Test]
    public void MustBeValidPreviousId_Valid_Succeeds()
    {
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase(prevId: Guid.NewGuid().ToString());

      validator.MustBeValidPreviousId();
      var valres = validator.Validate(evidence);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void MustBeValidPreviousId_Null_Succeeds()
    {
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase(prevId: null);

      validator.MustBeValidPreviousId();
      var valres = validator.Validate(evidence);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void MustBeValidPreviousId_NotGuid_ReturnsError()
    {
      var validator = new DummyEvidenceValidatorBase(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase(prevId: "not a GUID");

      validator.MustBeValidPreviousId();
      var valres = validator.Validate(evidence);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid PreviousId")
        .And
        .HaveCount(1);
    }
  }
}
