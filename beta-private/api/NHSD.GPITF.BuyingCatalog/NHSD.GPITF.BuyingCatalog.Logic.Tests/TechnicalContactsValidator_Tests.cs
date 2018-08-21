using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class TechnicalContactsValidator_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsDatastore> _solutionDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new TechnicalContactsValidator(_context.Object, _solutionDatastore.Object));
    }

    [Test]
    public void Validate_Buyer_ReturnsError(
      [ValueSource(nameof(Operations))] string operation)
    {
      var ctx = Creator.GetContext(role: Roles.Buyer);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var techCont = Creator.GetTechnicalContact();
      var validator = new TechnicalContactsValidator(_context.Object, _solutionDatastore.Object);

      var res = validator.Validate(techCont, ruleSet: operation);

      res.Errors.Count().Should().Be(2);
    }

    [Test]
    public void Validate_Admin_Succeeds(
      [ValueSource(nameof(Operations))] string operation)
    {
      var ctx = Creator.GetContext(role: Roles.Admin);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var techCont = Creator.GetTechnicalContact();
      var validator = new TechnicalContactsValidator(_context.Object, _solutionDatastore.Object);

      var res = validator.Validate(techCont, ruleSet: operation);

      res.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Supplier_Own_Succeeds(
      [ValueSource(nameof(Operations))] string operation)
    {
      var orgId = Guid.NewGuid().ToString();
      var ctx = Creator.GetContext(orgId: orgId, role: Roles.Supplier);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var soln = Creator.GetSolution(orgId: orgId);
      var techCont = Creator.GetTechnicalContact();
      _solutionDatastore.Setup(x => x.ById(techCont.SolutionId)).Returns(soln);
      var validator = new TechnicalContactsValidator(_context.Object, _solutionDatastore.Object);

      var res = validator.Validate(techCont, ruleSet: operation);

      res.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Supplier_Other_ReturnsError(
      [ValueSource(nameof(Operations))] string operation)
    {
      var orgId = Guid.NewGuid().ToString();
      var ctx = Creator.GetContext(orgId: orgId, role: Roles.Supplier);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var soln = Creator.GetSolution();
      var techCont = Creator.GetTechnicalContact();
      _solutionDatastore.Setup(x => x.ById(techCont.SolutionId)).Returns(soln);
      var validator = new TechnicalContactsValidator(_context.Object, _solutionDatastore.Object);

      var res = validator.Validate(techCont, ruleSet: operation);

      res.Errors.Count().Should().Be(1);
    }

    public static IEnumerable<string> Operations()
    {
      yield return nameof(ITechnicalContactsLogic.Create);
      yield return nameof(ITechnicalContactsLogic.Update);
      yield return nameof(ITechnicalContactsLogic.Delete);
    }
  }
}
