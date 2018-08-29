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
  public sealed class FrameworksFilter_Tests
  {
    private Mock<IHttpContextAccessor> _context;
    private Mock<ISolutionsDatastore> _solutionDatastore;
    private Mock<IFrameworksDatastore> _frameworkDatastore;

    [SetUp]
    public void SetUp()
    {
      _context = new Mock<IHttpContextAccessor>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
      _frameworkDatastore = new Mock<IFrameworksDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new FrameworksFilter(_context.Object, _solutionDatastore.Object, _frameworkDatastore.Object));
    }

    [TestCase(Roles.Admin)]
    [TestCase(Roles.Buyer)]
    public void Filter_NonSupplier_Returns_All(string role)
    {
      var ctx = Creator.GetContext(role: role);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new FrameworksFilter(_context.Object, _solutionDatastore.Object, _frameworkDatastore.Object);
      var frameworks = new[]
      {
        GetFramework(),
        GetFramework(),
        GetFramework()
      };
      var res = filter.Filter(frameworks);

      res.Should().BeEquivalentTo(frameworks);
    }

    [Test]
    public void Filter_Supplier_Returns_NotOther()
    {
      var orgId = Guid.NewGuid().ToString();
      var ctx = Creator.GetContext(orgId: orgId, role: Roles.Supplier);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new FrameworksFilter(_context.Object, _solutionDatastore.Object, _frameworkDatastore.Object);
      var fw1 = GetFramework();
      var fw2 = GetFramework();
      var fw3 = GetFramework();
      var frameworks = new[] { fw1, fw2, fw3 };
      var soln1 = Creator.GetSolution(orgId: orgId);
      var soln2 = Creator.GetSolution(orgId: orgId);
      var soln3 = Creator.GetSolution();
      _solutionDatastore
        .Setup(x => x.ByOrganisation(orgId))
        .Returns(new[] { soln1, soln2 });
      _frameworkDatastore
        .Setup(x => x.BySolution(soln1.Id))
        .Returns(new[] { fw1 });
      _frameworkDatastore
        .Setup(x => x.BySolution(soln2.Id))
        .Returns(new[] { fw1, fw2 });
      _frameworkDatastore
        .Setup(x => x.BySolution(soln3.Id))
        .Returns(new[] { fw1, fw2, fw3 });

      var res = filter.Filter(frameworks);

      res.Should().BeEquivalentTo(new[] { fw1, fw2 });
    }

    private static Frameworks GetFramework(
      string id = null)
    {
      return new Frameworks
      {
        Id = id ?? Guid.NewGuid().ToString()
      };
    }
  }
}
