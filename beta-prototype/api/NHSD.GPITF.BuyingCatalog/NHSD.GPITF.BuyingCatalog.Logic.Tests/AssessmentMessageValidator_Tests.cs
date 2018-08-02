using FluentAssertions;
using FluentValidation;
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
  public sealed class AssessmentMessageValidator_Tests
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
      Assert.DoesNotThrow(() => new AssessmentMessageValidator(_context.Object));
    }

    [Test]
    public void Validate_Admin_AllOperations_ReturnsNoError(
      [ValueSource(nameof(Operations))] string operation
      )
    {
      var ctx = Creator.GetContext(role: Roles.Admin);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new AssessmentMessageValidator(_context.Object);

      var res = validator.Validate(Creator.GetAssessmentMessage(), ruleSet: operation);

      res.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_AdminSupplier_BySolutionCreate_ReturnsNoError(
      [Values(nameof(IAssessmentMessageLogic.BySolution), nameof(IAssessmentMessageLogic.Create))] string operation,
      [Values(Roles.Admin, Roles.Supplier)] string role
      )
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new AssessmentMessageValidator(_context.Object);

      var res = validator.Validate(Creator.GetAssessmentMessage(), ruleSet: operation);

      res.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_Buyer_AllOperations_ReturnsError(
      [ValueSource(nameof(Operations))] string operation
      )
    {
      var ctx = Creator.GetContext(role: Roles.Buyer);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var validator = new AssessmentMessageValidator(_context.Object);

      var res = validator.Validate(Creator.GetAssessmentMessage(), ruleSet: operation);

      res.Errors.Count().Should().Be(1);
    }

    public static IEnumerable<string> Operations()
    {
      yield return nameof(IAssessmentMessageLogic.BySolution);
      yield return nameof(IAssessmentMessageLogic.Create);
      yield return nameof(IAssessmentMessageLogic.Update);
      yield return nameof(IAssessmentMessageLogic.Delete);
    }
  }
}
