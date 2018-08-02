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
  public sealed class TechnicalContactFilter_Tests
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
      Assert.DoesNotThrow(() => new TechnicalContactFilter(_context.Object, _solutionDatastore.Object));
    }

    [TestCase(Roles.Admin)]
    [TestCase(Roles.Buyer)]
    public void Filter_NonSupplier_Returns_All(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new TechnicalContactFilter(_context.Object, _solutionDatastore.Object);
      var techConts = new[]
      {
        Creator.GetTechnicalContact(),
        Creator.GetTechnicalContact(),
        Creator.GetTechnicalContact()
      };
      var res = filter.Filter(techConts.AsQueryable());

      res.Should().BeEquivalentTo(techConts);
    }

    [Test]
    public void Filter_Supplier_Returns_Own()
    {
      var orgId = Guid.NewGuid().ToString();
      var ctx = Creator.GetContext(orgId: orgId, role: Roles.Supplier);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new TechnicalContactFilter(_context.Object, _solutionDatastore.Object);
      var soln1 = Creator.GetSolution(orgId: orgId);
      var soln2 = Creator.GetSolution();
      var soln3 = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ById(soln1.Id)).Returns(soln1);
      _solutionDatastore.Setup(x => x.ById(soln2.Id)).Returns(soln2);
      _solutionDatastore.Setup(x => x.ById(soln3.Id)).Returns(soln3);
      var techContCtx1 = Creator.GetTechnicalContact(solutionId: soln1.Id);
      var techContCtx2 = Creator.GetTechnicalContact(solutionId: soln2.Id);
      var techContCtx3 = Creator.GetTechnicalContact(solutionId: soln3.Id);
      var techContCtxs = new[] { techContCtx1, techContCtx2, techContCtx3 };

      var res = filter.Filter(techContCtxs.AsQueryable());

      res.Should().BeEquivalentTo(new[] { techContCtx1 });
    }
  }
}
