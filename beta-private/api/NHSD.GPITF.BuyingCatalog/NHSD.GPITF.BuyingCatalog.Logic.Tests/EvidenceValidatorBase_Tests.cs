using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class EvidenceValidatorBase_Tests
  {
    private Mock<IHttpContextAccessor> _context;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyEvidenceValidatorBase(_context.Object));
    }

    [Test]
    public void MustBeValidClaimId_Valid_Succeeds()
    {
      var validator = new DummyEvidenceValidatorBase(_context.Object);
      var evidence = Creator.GetEvidenceBase();

      validator.MustBeValidClaimId();
      var valres = validator.Validate(evidence);

      valres.Errors.Should().BeEmpty();
    }

    [Test]
    public void MustBeValidClaimId_Null_ReturnsError()
    {
      var validator = new DummyEvidenceValidatorBase(_context.Object);
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
      var validator = new DummyEvidenceValidatorBase(_context.Object);
      var evidence = Creator.GetEvidenceBase(claimId: "some other Id");

      validator.MustBeValidClaimId();
      var valres = validator.Validate(evidence);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Invalid ClaimId")
        .And
        .HaveCount(1);
    }
  }
}
