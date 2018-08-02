using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  [TestFixture]
  public class SolutionFilter_Tests
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
      Assert.DoesNotThrow(() => new SolutionFilter(_context.Object));
    }

    [Test]
    public void Filter_Admin_Returns_All([ValueSource(nameof(Statuses))]SolutionStatus status)
    {
      var ctx = Creator.GetContext(role: Roles.Admin);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new SolutionFilter(_context.Object);
      var solns = new[]
      {
        Creator.GetSolution(status: status),
        Creator.GetSolution(status: status),
        Creator.GetSolution(status: status)
      };

      var res = filter.Filter(solns.AsQueryable());

      res.Should().BeEquivalentTo(solns);
    }

    [Test]
    public void Filter_Buyer_Returns_NonDraft([ValueSource(nameof(Statuses))]SolutionStatus status)
    {
      var ctx = Creator.GetContext(role: Roles.Buyer);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new SolutionFilter(_context.Object);
      var solns = new[]
      {
        Creator.GetSolution(status: status),
        Creator.GetSolution(status: status),
        Creator.GetSolution(status: status)
      };
      var expSolns = solns.Where(x => x.Status != SolutionStatus.Draft);

      var res = filter.Filter(solns.AsQueryable());

      res.Should().BeEquivalentTo(expSolns);
    }

    [Test]
    public void Filter_None_Returns_NonDraft([ValueSource(nameof(Statuses))]SolutionStatus status)
    {
      var ctx = Creator.GetContext(role: "None");
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new SolutionFilter(_context.Object);
      var solns = new[]
      {
        Creator.GetSolution(status: status),
        Creator.GetSolution(status: status),
        Creator.GetSolution(status: status)
      };
      var expSolns = solns.Where(x => x.Status != SolutionStatus.Draft);

      var res = filter.Filter(solns.AsQueryable());

      res.Should().BeEquivalentTo(expSolns);
    }

    [Test]
    public void Filter_Supplier_Returns_Own([ValueSource(nameof(Statuses))]SolutionStatus status)
    {
      var orgId = Guid.NewGuid().ToString();
      var ctx = Creator.GetContext(orgId: orgId, role: Roles.Supplier);
      _context.Setup(c => c.HttpContext).Returns(ctx);
      var filter = new SolutionFilter(_context.Object);
      var soln1 = Creator.GetSolution(status: status, orgId: orgId);
      var soln2 = Creator.GetSolution(status: status);
      var soln3 = Creator.GetSolution(status: status);
      var solns = new[] { soln1, soln2, soln3 };

      var res = filter.Filter(solns.AsQueryable());

      res.Should().BeEquivalentTo(soln1);
    }

    public static IEnumerable<SolutionStatus> Statuses()
    {
      return (SolutionStatus[])Enum.GetValues(typeof(SolutionStatus));
    }
  }
}
