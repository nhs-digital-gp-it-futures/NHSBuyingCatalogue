using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class SolutionsLogic_Tests
  {
    private Mock<ISolutionsDatastore> _datastore;
    private Mock<IContactsDatastore> _contacts;
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsValidator> _validator;
    private Mock<ISolutionsFilter> _filter;
    private Mock<IEvidenceBlobStoreLogic> _evidenceBlobStoreLogic;

    [SetUp]
    public void SetUp()
    {
      _datastore = new Mock<ISolutionsDatastore>();
      _contacts = new Mock<IContactsDatastore>();
      _context = new Mock<IHttpContextAccessor>();
      _validator = new Mock<ISolutionsValidator>();
      _filter = new Mock<ISolutionsFilter>();
      _evidenceBlobStoreLogic = new Mock<IEvidenceBlobStoreLogic>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object, _evidenceBlobStoreLogic.Object));
    }

    [Test]
    public void ByFramework_CallsFilter()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object, _evidenceBlobStoreLogic.Object);

      logic.ByFramework("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Once());
    }

    [Test]
    public void ById_CallsFilter()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object, _evidenceBlobStoreLogic.Object);

      logic.ById("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Once());
    }

    [Test]
    public void ByOrganisation_CallsFilter()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object, _evidenceBlobStoreLogic.Object);

      logic.ByOrganisation("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Once());
    }

    [Test]
    public void Create_CallsValidator_WithRuleset()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Create(soln);

      // obfuscated validation code because we use an extension method
      _validator.Verify(x => x.Validate(It.Is<ValidationContext>(
        vc =>
          vc.InstanceToValidate == soln &&
          vc.Selector is RulesetValidatorSelector &&
          ((RulesetValidatorSelector)vc.Selector).RuleSets.Contains(nameof(ISolutionsLogic.Create)))), Times.Once());
    }

    [Test]
    public void Update_CallsValidator_WithRuleset()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution();
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      // obfuscated validation code because we use an extension method
      _validator.Verify(x => x.Validate(It.Is<ValidationContext>(
        vc =>
          vc.InstanceToValidate == soln &&
          vc.Selector is RulesetValidatorSelector &&
          ((RulesetValidatorSelector)vc.Selector).RuleSets.Contains(nameof(ISolutionsLogic.Update)))), Times.Once());
    }

    [TestCase(SolutionStatus.CapabilitiesAssessment)]
    public void Update_CallsPrepareForSolution_WhenCapabilitiesAssessment(SolutionStatus status)
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution(status: status);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _evidenceBlobStoreLogic.Verify(x => x.PrepareForSolution(soln.Id), Times.Once);
    }

    [TestCase(SolutionStatus.Failed)]
    [TestCase(SolutionStatus.Draft)]
    [TestCase(SolutionStatus.Registered)]
    [TestCase(SolutionStatus.StandardsCompliance)]
    [TestCase(SolutionStatus.FinalApproval)]
    [TestCase(SolutionStatus.SolutionPage)]
    [TestCase(SolutionStatus.Approved)]
    public void Update_DoesNotCallPrepareForSolution_WhenNotCapabilitiesAssessment(SolutionStatus status)
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object, _evidenceBlobStoreLogic.Object);
      var soln = Creator.GetSolution(status: status);
      _context.Setup(x => x.HttpContext).Returns(Creator.GetContext());
      _contacts.Setup(x => x.ByEmail(It.IsAny<string>())).Returns(Creator.GetContact());

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(soln);

      _evidenceBlobStoreLogic.Verify(x => x.PrepareForSolution(soln.Id), Times.Never);
    }
  }
}
