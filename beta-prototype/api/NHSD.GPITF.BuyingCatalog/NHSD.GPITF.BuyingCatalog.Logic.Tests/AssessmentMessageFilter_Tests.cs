using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class AssessmentMessageFilter_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionDatastore> _solutionDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _solutionDatastore = new Mock<ISolutionDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new AssessmentMessageFilter(_context.Object, _solutionDatastore.Object));
    }

    [Test]
    public void Filter_Admin_ReturnsAll()
    {
      var ctx = Creator.GetContext(role: Roles.Admin);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new AssessmentMessageFilter(_context.Object, _solutionDatastore.Object);
      var assMesses = new[]
      {
        Creator.GetAssessmentMessage(),
        Creator.GetAssessmentMessage(),
        Creator.GetAssessmentMessage()
      };

      var res = filter.Filter(assMesses.AsQueryable());

      res.Should().BeEquivalentTo(assMesses);
    }

    [Test]
    public void Filter_Buyer_ReturnsEmpty()
    {
      var ctx = Creator.GetContext(role: Roles.Buyer);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new AssessmentMessageFilter(_context.Object, _solutionDatastore.Object);
      var assMesses = new[]
      {
        Creator.GetAssessmentMessage(),
        Creator.GetAssessmentMessage(),
        Creator.GetAssessmentMessage()
      };

      var res = filter.Filter(assMesses.AsQueryable());

      res.Should().BeEmpty();
    }

    [Test]
    public void Filter_Supplier_ReturnsOwn()
    {
      var orgId = Guid.NewGuid().ToString();
      var ctx = Creator.GetContext(orgId: orgId, role: Roles.Supplier);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new AssessmentMessageFilter(_context.Object, _solutionDatastore.Object);
      var assMess1 = Creator.GetAssessmentMessage();
      var assMess2 = Creator.GetAssessmentMessage();
      var assMess3 = Creator.GetAssessmentMessage();
      var assMesses = new[] { assMess1, assMess2, assMess3 };
      var soln = Creator.GetSolution(orgId: orgId);
      _solutionDatastore.Setup(x => x.ById(assMess1.SolutionId)).Returns(soln);

      var res = filter.Filter(assMesses.AsQueryable());

      res.Should().BeEquivalentTo(assMess1);
    }
  }
}
