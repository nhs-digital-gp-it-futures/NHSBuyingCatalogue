using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NHSD.GPITF.BuyingCatalog.Logic.Porcelain;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests.Porcelain
{
  [TestFixture]
  public sealed class SolutionsExLogic_Tests
  {
    private Mock<ISolutionsModifier> _solutionsModifier;
    private Mock<ISolutionsExDatastore> _datastore;
    private Mock<IContactsDatastore> _contacts;
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsExValidator> _validator;
    private Mock<ISolutionsExFilter> _filter;
    private Mock<IEvidenceBlobStoreLogic> _evidenceBlobStoreLogic;

    [SetUp]
    public void SetUp()
    {
      _solutionsModifier = new Mock<ISolutionsModifier>();
      _datastore = new Mock<ISolutionsExDatastore>();
      _contacts = new Mock<IContactsDatastore>();
      _context = new Mock<IHttpContextAccessor>();
      _validator = new Mock<ISolutionsExValidator>();
      _filter = new Mock<ISolutionsExFilter>();
      _evidenceBlobStoreLogic = new Mock<IEvidenceBlobStoreLogic>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object));
    }

    [Test]
    public void Update_CallsValidator_WithRuleset()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var solnEx = Creator.GetSolutionEx();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _validator.Verify(x => x.ValidateAndThrowEx(
        It.Is<SolutionEx>(sex => sex == solnEx),
        It.Is<string>(rs => rs == nameof(ISolutionsExLogic.Update))), Times.Once());
    }

    [TestCase(SolutionStatus.Registered)]
    public void Update_CallsPrepareForSolution_WhenRegistered(SolutionStatus status)
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution(status: status);
      var solnEx = Creator.GetSolutionEx(soln: soln);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _evidenceBlobStoreLogic.Verify(x => x.PrepareForSolution(soln.Id), Times.Once);
    }

    [TestCase(SolutionStatus.Failed)]
    [TestCase(SolutionStatus.Draft)]
    [TestCase(SolutionStatus.CapabilitiesAssessment)]
    [TestCase(SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.Approved)]
    public void Update_DoesNotCallPrepareForSolution_WhenNotRegistered(SolutionStatus status)
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution(status: status);
      var solnEx = Creator.GetSolutionEx(soln: soln);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _evidenceBlobStoreLogic.Verify(x => x.PrepareForSolution(soln.Id), Times.Never);
    }

    [Test]
    public void Update_Calls_SolutionModifier()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      _solutionsModifier.Verify(x => x.ForUpdate(soln), Times.Once);
    }

    [Test]
    public void Update_Sets_ClaimedCapabilityReview_OriginalDate_WhenNotSet()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var review = Creator.GetCapabilitiesImplementedReviews();
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedCapRev: new List<CapabilitiesImplementedReviews>(new[] { review }));
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      review.OriginalDate.Should().BeCloseTo(DateTime.UtcNow);
    }

    [Test]
    public void Update_DoesNotSet_ClaimedCapabilityReview_OriginalDate_WhenSet()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var originalDate = new DateTime(2006, 2, 20);
      var review = Creator.GetCapabilitiesImplementedReviews(originalDate: originalDate);
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedCapRev: new List<CapabilitiesImplementedReviews>(new[] { review }));
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      review.OriginalDate.Should().BeCloseTo(originalDate);
    }

    [Test]
    public void Update_Sets_ClaimedStandardReview_OriginalDate_WhenNotSet()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var review = Creator.GetStandardsApplicableReviews();
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedStdRev: new List<StandardsApplicableReviews>(new[] { review }));
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      review.OriginalDate.Should().BeCloseTo(DateTime.UtcNow);
    }

    [Test]
    public void Update_DoesNotSet_ClaimedStandardReview_OriginalDate_WhenSet()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var originalDate = new DateTime(2006, 2, 20);
      var review = Creator.GetStandardsApplicableReviews(originalDate: originalDate);
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedStdRev: new List<StandardsApplicableReviews>(new[] { review }));
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      review.OriginalDate.Should().BeCloseTo(originalDate);
    }

    [Test]
    public void Update_Sets_ClaimedCapabilityEvidence_OriginalDate_WhenNotSet()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var evidence = Creator.GetCapabilitiesImplementedEvidence();
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedCapEv: new List<CapabilitiesImplementedEvidence>(new[] { evidence }));
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      evidence.OriginalDate.Should().BeCloseTo(DateTime.UtcNow);
    }

    [Test]
    public void Update_DoesNotSet_ClaimedCapabilityEvidence_OriginalDate_WhenSet()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var originalDate = new DateTime(2006, 2, 20);
      var evidence = Creator.GetCapabilitiesImplementedEvidence(originalDate: originalDate);
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedCapEv: new List<CapabilitiesImplementedEvidence>(new[] { evidence }));
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      evidence.OriginalDate.Should().BeCloseTo(originalDate);
    }

    [Test]
    public void Update_Sets_ClaimedStandardEvidence_OriginalDate_WhenNotSet()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var evidence = Creator.GetStandardsApplicableEvidence();
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedStdEv: new List<StandardsApplicableEvidence>(new[] { evidence }));
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      evidence.OriginalDate.Should().BeCloseTo(DateTime.UtcNow);
    }

    [Test]
    public void Update_DoesNotSet_ClaimedStandardEvidence_OriginalDate_WhenSet()
    {
      var logic = new SolutionsExLogic(_solutionsModifier.Object, _datastore.Object, _context.Object, _validator.Object, _filter.Object, _contacts.Object, _evidenceBlobStoreLogic.Object);
      var originalDate = new DateTime(2006, 2, 20);
      var evidence = Creator.GetStandardsApplicableEvidence(originalDate: originalDate);
      var soln = Creator.GetSolution();
      var solnEx = Creator.GetSolutionEx(soln: soln, claimedStdEv: new List<StandardsApplicableEvidence>(new[] { evidence }));
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      var contact = Creator.GetContact();
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(contact);

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(solnEx);

      evidence.OriginalDate.Should().BeCloseTo(originalDate);
    }
  }
}
