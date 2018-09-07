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
  public class SolutionsLogic_Tests
  {
    private Mock<ISolutionsDatastore> _datastore;
    private Mock<IContactsDatastore> _contacts;
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsValidator> _validator;
    private Mock<ISolutionsFilter> _filter;

    [SetUp]
    public void SetUp()
    {
      _datastore = new Mock<ISolutionsDatastore>();
      _contacts = new Mock<IContactsDatastore>();
      _context = new Mock<IHttpContextAccessor>();
      _validator = new Mock<ISolutionsValidator>();
      _filter = new Mock<ISolutionsFilter>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object));
    }

    [Test]
    public void ByFramework_CallsFilter()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object);

      logic.ByFramework("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Once());
    }

    [Test]
    public void ById_CallsFilter()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object);

      logic.ById("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Once());
    }

    [Test]
    public void ByOrganisation_CallsFilter()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object);

      logic.ByOrganisation("some Id");

      _filter.Verify(x => x.Filter(It.IsAny<IEnumerable<Solutions>>()), Times.Once());
    }

    [Test]
    public void Create_CallsValidator_WithoutRuleset()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object);
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
          vc.Selector is DefaultValidatorSelector)), Times.Once());
    }

    [Test]
    public void Create_CallsValidator_WithRuleset()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object);
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
    public void Update_CallsValidator_WithoutRuleset()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object);
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
          vc.Selector is DefaultValidatorSelector)), Times.Once());
    }

    [Test]
    public void Update_CallsValidator_WithRuleset()
    {
      var logic = new SolutionsLogic(_datastore.Object, _contacts.Object, _context.Object, _validator.Object, _filter.Object);
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
  }
}
