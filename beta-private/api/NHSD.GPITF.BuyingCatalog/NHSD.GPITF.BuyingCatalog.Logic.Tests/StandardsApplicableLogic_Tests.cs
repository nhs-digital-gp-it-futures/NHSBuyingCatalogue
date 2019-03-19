using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class StandardsApplicableLogic_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<IStandardsApplicableDatastore> _datastore;
    private Mock<IStandardsApplicableValidator> _validator;
    private Mock<IStandardsApplicableFilter> _filter;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _datastore = new Mock<IStandardsApplicableDatastore>();
      _validator = new Mock<IStandardsApplicableValidator>();
      _filter    = new Mock<IStandardsApplicableFilter>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new StandardsApplicableLogic(_datastore.Object, _validator.Object, _filter.Object, _context.Object));
    }

    [Test]
    public void Update_CallsValidator_WithRuleset()
    {
      var logic = new StandardsApplicableLogic(_datastore.Object, _validator.Object, _filter.Object, _context.Object);
      var claim = Creator.GetStandardsApplicable();

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(claim);

      _validator.Verify(x => x.ValidateAndThrowEx(
        It.Is<StandardsApplicable>(c => c == claim),
        It.Is<string>(rs => rs == nameof(IClaimsLogic<StandardsApplicable>.Update))), Times.Once());
    }

    [Test]
    public void Update_ToSubmitted_Sets_SubmittedOn_ToUtcNow()
    {
      var logic = new StandardsApplicableLogic(_datastore.Object, _validator.Object, _filter.Object, _context.Object);
      var submittedOn = new DateTime(2006, 2, 20, 6, 3, 0);
      var claim = Creator.GetStandardsApplicable(status: StandardsApplicableStatus.Submitted, submittedOn: submittedOn);
      _datastore.Setup(x => x.Create(claim)).Returns(claim);
      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(claim);

      claim.SubmittedOn.Should().BeCloseTo(DateTime.UtcNow);
    }

    [TestCase(StandardsApplicableStatus.NotStarted)]
    [TestCase(StandardsApplicableStatus.Draft)]
    [TestCase(StandardsApplicableStatus.Remediation)]
    [TestCase(StandardsApplicableStatus.Approved)]
    [TestCase(StandardsApplicableStatus.ApprovedFirstOfType)]
    [TestCase(StandardsApplicableStatus.ApprovedPartial)]
    [TestCase(StandardsApplicableStatus.Rejected)]
    public void Update_NotSubmitted_DoesNotSet_SubmittedOn(StandardsApplicableStatus status)
    {
      var logic = new StandardsApplicableLogic(_datastore.Object, _validator.Object, _filter.Object, _context.Object);
      var submittedOn = new DateTime(2006, 2, 20, 6, 3, 0);
      var claim = Creator.GetStandardsApplicable(status: status, submittedOn: submittedOn);
      _datastore.Setup(x => x.Create(claim)).Returns(claim);
      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(claim);

      claim.SubmittedOn.Should().BeCloseTo(submittedOn);
    }
  }
}
