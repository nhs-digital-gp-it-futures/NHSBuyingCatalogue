using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public sealed class FrameworkFilter_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionDatastore> _solutionDatastore;
    private Mock<IFrameworkDatastore> _frameworkDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _solutionDatastore = new Mock<ISolutionDatastore>();
      _frameworkDatastore = new Mock<IFrameworkDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new FrameworkFilter(_context.Object, _solutionDatastore.Object, _frameworkDatastore.Object));
    }

    [TestCase(Roles.Admin)]
    [TestCase(Roles.Buyer)]
    public void Filter_NonSupplier_Returns_All(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new FrameworkFilter(_context.Object, _solutionDatastore.Object, _frameworkDatastore.Object);
      var frameworks = new[]
      {
        GetFramework(),
        GetFramework(),
        GetFramework()
      };
      var res = filter.Filter(frameworks.AsQueryable());

      res.Should().BeEquivalentTo(frameworks);
    }

    [Test]
    public void Filter_Supplier_Returns_NotOther()
    {
      var orgId = Guid.NewGuid().ToString();
      var ctx = Creator.GetContext(orgId: orgId, role: Roles.Supplier);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new FrameworkFilter(_context.Object, _solutionDatastore.Object, _frameworkDatastore.Object);
      var fw1 = GetFramework();
      var fw2 = GetFramework();
      var fw3 = GetFramework();
      var frameworks = new[] { fw1, fw2, fw3 };
      var soln1 = Creator.GetSolution(orgId: orgId);
      var soln2 = Creator.GetSolution(orgId: orgId);
      var soln3 = Creator.GetSolution();
      _solutionDatastore
        .Setup(x => x.ByOrganisation(orgId))
        .Returns(new[] { soln1, soln2 }.AsQueryable());
      _frameworkDatastore
        .Setup(x => x.BySolution(soln1.Id))
        .Returns(new[] { fw1 }.AsQueryable());
      _frameworkDatastore
        .Setup(x => x.BySolution(soln2.Id))
        .Returns(new[] { fw1, fw2 }.AsQueryable());
      _frameworkDatastore
        .Setup(x => x.BySolution(soln3.Id))
        .Returns(new[] { fw1, fw2, fw3 }.AsQueryable());

      var res = filter.Filter(frameworks.AsQueryable());

      res.Should().BeEquivalentTo(new[] { fw1, fw2 });
    }

    private static Framework GetFramework(
      string id = null)
    {
      return new Framework
      {
        Id = id ?? Guid.NewGuid().ToString()
      };
    }
  }
}
