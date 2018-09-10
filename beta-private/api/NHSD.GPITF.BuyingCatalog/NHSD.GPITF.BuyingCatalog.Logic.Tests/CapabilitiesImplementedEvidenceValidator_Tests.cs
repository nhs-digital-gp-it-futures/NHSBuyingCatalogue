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
  public sealed class CapabilitiesImplementedEvidenceValidator_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ICapabilitiesImplementedEvidenceDatastore> _evidenceDatastore;
    private Mock<ICapabilitiesImplementedDatastore> _claimDatastore;
    private Mock<ISolutionsDatastore> _solutionDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _evidenceDatastore = new Mock<ICapabilitiesImplementedEvidenceDatastore>();
      _claimDatastore = new Mock<ICapabilitiesImplementedDatastore>();
      _claimDatastore.As<IClaimsDatastore<ClaimsBase>>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new CapabilitiesImplementedEvidenceValidator(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object));
    }

    [TestCase(SolutionStatus.CapabilitiesAssessment)]
    public void SolutionMustBeInReview_Review_Succeeds(SolutionStatus status)
    {
      var validator = new CapabilitiesImplementedEvidenceValidator(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var soln = Creator.GetSolution(status: status);
      var claim = Creator.GetCapabilitiesImplemented(solnId: soln.Id);
      var evidence = GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      _claimDatastore.As<IClaimsDatastore<ClaimsBase>>().Setup(x => x.ById(evidence.ClaimId)).Returns(claim);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      validator.SolutionMustBeInReview();
      var valres = validator.Validate(evidence);

      valres.Errors.Should().BeEmpty();
    }

    [TestCase(SolutionStatus.Failed)]
    [TestCase(SolutionStatus.Draft)]
    [TestCase(SolutionStatus.Registered)]
    [TestCase(SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.Approved)]
    public void SolutionMustBeInReview_NonReview_ReturnsError(SolutionStatus status)
    {
      var validator = new CapabilitiesImplementedEvidenceValidator(_evidenceDatastore.Object, _claimDatastore.Object, _solutionDatastore.Object, _context.Object);
      var soln = Creator.GetSolution(status: status);
      var claim = Creator.GetCapabilitiesImplemented(solnId: soln.Id);
      var evidence = GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      _claimDatastore.As<IClaimsDatastore<ClaimsBase>>().Setup(x => x.ById(evidence.ClaimId)).Returns(claim);
      _solutionDatastore.Setup(x => x.ById(soln.Id)).Returns(soln);

      validator.SolutionMustBeInReview();
      var valres = validator.Validate(evidence);

      valres.Errors.Should()
        .ContainSingle(x => x.ErrorMessage == "Can only add evidence if solution is in review")
        .And
        .HaveCount(1);
    }

    private static CapabilitiesImplementedEvidence GetCapabilitiesImplementedEvidence(
      string id = null,
      string prevId = null,
      string claimId = null)
    {
      return new CapabilitiesImplementedEvidence
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        ClaimId = claimId ?? Guid.NewGuid().ToString()
      };
    }
  }
}
