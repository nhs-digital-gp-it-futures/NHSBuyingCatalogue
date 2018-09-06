﻿using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class ClaimsLogicBase_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<IClaimsDatastore<ClaimsBase>> _datastore;
    private Mock<IClaimsValidator<ClaimsBase>> _validator;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _datastore = new Mock<IClaimsDatastore<ClaimsBase>>();
      _validator = new Mock<IClaimsValidator<ClaimsBase>>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _context.Object));
    }

    [Test]
    public void Create_CallsValidator_WithoutRuleset()
    {
      var logic = new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _context.Object);
      var claim = Creator.GetClaimsBase();

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Create(claim);

      // obfuscated validation code because we use an extension method
      _validator.Verify(x => x.Validate(It.Is<ValidationContext>(
        vc =>
          vc.InstanceToValidate == claim &&
          vc.Selector is DefaultValidatorSelector)), Times.Once());
    }

    [Test]
    public void Create_CallsValidator_WithRuleset()
    {
      var logic = new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _context.Object);
      var claim = Creator.GetClaimsBase();

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Create(claim);

      // obfuscated validation code because we use an extension method
      _validator.Verify(x => x.Validate(It.Is<ValidationContext>(
        vc =>
          vc.InstanceToValidate == claim &&
          vc.Selector is RulesetValidatorSelector &&
          ((RulesetValidatorSelector)vc.Selector).RuleSets.Contains(nameof(IClaimsLogic<ClaimsBase>.Create)))), Times.Once());
    }

    [Test]
    public void Update_CallsValidator_WithoutRuleset()
    {
      var logic = new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _context.Object);
      var claim = Creator.GetClaimsBase();

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(claim);

      _validator.Verify(x => x.Validate(It.Is<ValidationContext>(
        vc =>
          vc.InstanceToValidate == claim &&
          vc.Selector is DefaultValidatorSelector)), Times.Once());
    }

    [Test]
    public void Update_CallsValidator_WithRuleset()
    {
      var logic = new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _context.Object);
      var claim = Creator.GetClaimsBase();

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Update(claim);

      _validator.Verify(x => x.Validate(It.Is<ValidationContext>(
        vc =>
          vc.InstanceToValidate == claim &&
          vc.Selector is RulesetValidatorSelector &&
          ((RulesetValidatorSelector)vc.Selector).RuleSets.Contains(nameof(IClaimsLogic<ClaimsBase>.Update)))), Times.Once());
    }

    [Test]
    public void Delete_CallsValidator_WithoutRuleset()
    {
      var logic = new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _context.Object);
      var claim = Creator.GetClaimsBase();

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Delete(claim);

      _validator.Verify(x => x.Validate(It.Is<ValidationContext>(
        vc =>
          vc.InstanceToValidate == claim &&
          vc.Selector is DefaultValidatorSelector)), Times.Once());
    }

    [Test]
    public void Delete_CallsValidator_WithRuleset()
    {
      var logic = new DummyClaimsLogicBase(_datastore.Object, _validator.Object, _context.Object);
      var claim = Creator.GetClaimsBase();

      var valres = new ValidationResult();
      _validator.Setup(x => x.Validate(It.IsAny<ValidationContext>())).Returns(valres);

      logic.Delete(claim);

      _validator.Verify(x => x.Validate(It.Is<ValidationContext>(
        vc =>
          vc.InstanceToValidate == claim &&
          vc.Selector is RulesetValidatorSelector &&
          ((RulesetValidatorSelector)vc.Selector).RuleSets.Contains(nameof(IClaimsLogic<ClaimsBase>.Delete)))), Times.Once());
    }
  }
}
