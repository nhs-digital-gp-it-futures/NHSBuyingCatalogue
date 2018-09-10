using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class EvidenceValidatorBase_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<IStandardsApplicableDatastore> _claimDatastore;
    private Mock<ISolutionsDatastore> _solutionDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _claimDatastore = new Mock<IStandardsApplicableDatastore>();
      _claimDatastore.As<IClaimsDatastore<ClaimsBase>>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyEvidenceValidatorBase(_claimDatastore.Object, _solutionDatastore.Object, _context.Object));
    }

    [Test]
    public void MustBeValidClaimId_Valid_Succeeds()
    {
      var validator = new DummyEvidenceValidatorBase(_claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase();

      validator.MustBeValidClaimId();
      var valres = validator.Validate(evidence);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void MustBeValidClaimId_Null_ReturnsError()
    {
      var validator = new DummyEvidenceValidatorBase(_claimDatastore.Object, _solutionDatastore.Object, _context.Object);
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
      var validator = new DummyEvidenceValidatorBase(_claimDatastore.Object, _solutionDatastore.Object, _context.Object);
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
      var validator = new DummyEvidenceValidatorBase(_claimDatastore.Object, _solutionDatastore.Object, _context.Object);
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
      var validator = new DummyEvidenceValidatorBase(_claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var evidence = Creator.GetEvidenceBase();

      validator.MustBeSupplier();
      var valres = validator.Validate(evidence);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Must be supplier")
        .And
        .HaveCount(1);
    }
  }
}
