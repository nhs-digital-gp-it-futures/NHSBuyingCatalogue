using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Porcelain;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain;
using NUnit.Framework;

namespace NHSD.GPITF.BuyingCatalog.Search.Tests
{
  [TestFixture]
  public sealed class SearchDatastore_Tests
  {
    private Mock<IDbConnectionFactory> _dbConnectionFactory;
    private Mock<ILogger<SearchDatastore>> _logger;
    private Mock<ISyncPolicyFactory> _policy;
    private Mock<IFrameworksDatastore> _frameworkDatastore;
    private Mock<ISolutionsDatastore> _solutionDatastore;
    private Mock<ICapabilitiesDatastore> _capabilityDatastore;
    private Mock<ICapabilitiesImplementedDatastore> _claimedCapabilityDatastore;
    private Mock<ISolutionsExDatastore> _solutionExDatastore;

    [SetUp]
    public void SetUp()
    {
      _dbConnectionFactory = new Mock<IDbConnectionFactory>();
      _logger = new Mock<ILogger<SearchDatastore>>();
      _policy = new Mock<ISyncPolicyFactory>();
      _frameworkDatastore = new Mock<IFrameworksDatastore>();
      _solutionDatastore = new Mock<ISolutionsDatastore>();
      _capabilityDatastore = new Mock<ICapabilitiesDatastore>();
      _claimedCapabilityDatastore = new Mock<ICapabilitiesImplementedDatastore>();
      _solutionExDatastore = new Mock<ISolutionsExDatastore>();
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SearchDatastore(
        _dbConnectionFactory.Object,
        _logger.Object,
        _policy.Object,
        _frameworkDatastore.Object,
        _solutionDatastore.Object,
        _capabilityDatastore.Object,
        _claimedCapabilityDatastore.Object,
        _solutionExDatastore.Object));
    }

    [TestCase("Document Manager", "doc")]
    [TestCase("Document Manager", "manage")]
    [TestCase("Patient Collaboration", "collab")]
    [TestCase("Patient Collaboration", "ient")]
    public void ByKeyword_KeywordInCapabilityName_ReturnsSolution(
      string capabilityName,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability(name: capabilityName);
      _capabilityDatastore.Setup(x => x.ById(capability.Id)).Returns(capability);
      _capabilityDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { capability });

      var claimedCapability = Creator.GetClaimedCapability(solutionId: soln.Id, capabilityId: capability.Id);
      _claimedCapabilityDatastore.Setup(x => x.BySolution(soln.Id)).Returns(new[] { claimedCapability });

      var solnEx = Creator.GetSolutionEx(solution: soln);
      _solutionExDatastore.Setup(x => x.BySolution(soln.Id)).Returns(solnEx);

      var search = new SearchDatastore(
        _dbConnectionFactory.Object,
        _logger.Object,
        _policy.Object,
        _frameworkDatastore.Object,
        _solutionDatastore.Object,
        _capabilityDatastore.Object,
        _claimedCapabilityDatastore.Object,
        _solutionExDatastore.Object);

      var results = search.ByKeyword(keyword);

      var res = results.Should().ContainSingle();
      res.Which.SolutionEx.Should().BeEquivalentTo(solnEx);
      res.Which.Distance.Should().Be(0);
    }

    [TestCase("Does Really Kool document management", "doc")]
    [TestCase("Does Really Kool document management", "manage")]
    [TestCase("Does Really Kool patient collaboration", "collab")]
    [TestCase("Does Really Kool patient collaboration", "ient")]
    public void ByKeyword_KeywordInCapabilityDescription_ReturnsSolution(
      string capabilityDescription,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability(description: capabilityDescription);
      _capabilityDatastore.Setup(x => x.ById(capability.Id)).Returns(capability);
      _capabilityDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { capability });

      var claimedCapability = Creator.GetClaimedCapability(solutionId: soln.Id, capabilityId: capability.Id);
      _claimedCapabilityDatastore.Setup(x => x.BySolution(soln.Id)).Returns(new[] { claimedCapability });

      var solnEx = Creator.GetSolutionEx(solution: soln);
      _solutionExDatastore.Setup(x => x.BySolution(soln.Id)).Returns(solnEx);

      var search = new SearchDatastore(
        _dbConnectionFactory.Object,
        _logger.Object,
        _policy.Object,
        _frameworkDatastore.Object,
        _solutionDatastore.Object,
        _capabilityDatastore.Object,
        _claimedCapabilityDatastore.Object,
        _solutionExDatastore.Object);

      var results = search.ByKeyword(keyword);

      var res = results.Should().ContainSingle();

      res.Which.SolutionEx.Should().BeEquivalentTo(solnEx);
      res.Which.Distance.Should().Be(0);
    }

    [TestCase("Document Manager", "docs")]
    [TestCase("Document Manager", "manages")]
    [TestCase("Patient Collaboration", "collaborates")]
    [TestCase("Patient Collaboration", "sentient")]
    public void ByKeyword_KeywordNotInCapabilityName_ReturnsNone(
      string capabilityName,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability(name: capabilityName);
      _capabilityDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { capability });

      var claimedCapability = Creator.GetClaimedCapability(solutionId: soln.Id, capabilityId: capability.Id);
      _claimedCapabilityDatastore.Setup(x => x.BySolution(soln.Id)).Returns(new[] { claimedCapability });

      var solnEx = Creator.GetSolutionEx(solution: soln);
      _solutionExDatastore.Setup(x => x.BySolution(soln.Id)).Returns(solnEx);

      var search = new SearchDatastore(
        _dbConnectionFactory.Object,
        _logger.Object,
        _policy.Object,
        _frameworkDatastore.Object,
        _solutionDatastore.Object,
        _capabilityDatastore.Object,
        _claimedCapabilityDatastore.Object,
        _solutionExDatastore.Object);

      var results = search.ByKeyword(keyword);

      results.Should().BeEmpty();
    }

    [TestCase("Does Really Kool document management", "docs")]
    [TestCase("Does Really Kool document management", "manages")]
    [TestCase("Does Really Kool patient collaboration", "collaborates")]
    [TestCase("Does Really Kool patient collaboration", "sentient")]
    public void ByKeyword_KeywordNotInCapabilityDescription_ReturnsNone(
      string capabilityDescription,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability(description: capabilityDescription);
      _capabilityDatastore.Setup(x => x.ById(capability.Id)).Returns(capability);
      _capabilityDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { capability });

      var claimedCapability = Creator.GetClaimedCapability(solutionId: soln.Id, capabilityId: capability.Id);
      _claimedCapabilityDatastore.Setup(x => x.BySolution(soln.Id)).Returns(new[] { claimedCapability });

      var solnEx = Creator.GetSolutionEx(solution: soln);
      _solutionExDatastore.Setup(x => x.BySolution(soln.Id)).Returns(solnEx);

      var search = new SearchDatastore(
        _dbConnectionFactory.Object,
        _logger.Object,
        _policy.Object,
        _frameworkDatastore.Object,
        _solutionDatastore.Object,
        _capabilityDatastore.Object,
        _claimedCapabilityDatastore.Object,
        _solutionExDatastore.Object);

      var results = search.ByKeyword(keyword);

      results.Should().BeEmpty();
    }

    [TestCase("Does Really Kool document management", "doc")]
    [TestCase("Does Really Kool document management", "manage")]
    [TestCase("Does Really Kool patient collaboration", "collab")]
    [TestCase("Does Really Kool patient collaboration", "ient")]
    public void ByKeyword_SolutionMultiCapability_ReturnsSolution(
      string capabilityDescription,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln1 = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln1 });

      var cap1 = Creator.GetCapability(description: capabilityDescription);
      var cap2 = Creator.GetCapability();
      _capabilityDatastore.Setup(x => x.ById(cap1.Id)).Returns(cap1);
      _capabilityDatastore.Setup(x => x.ById(cap2.Id)).Returns(cap2);
      _capabilityDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { cap1, cap2 });

      var claimedCap11 = Creator.GetClaimedCapability(solutionId: soln1.Id, capabilityId: cap1.Id);
      var claimedCap12 = Creator.GetClaimedCapability(solutionId: soln1.Id, capabilityId: cap2.Id);
      _claimedCapabilityDatastore.Setup(x => x.BySolution(soln1.Id)).Returns(new[] { claimedCap11, claimedCap12 });

      var solnEx1 = Creator.GetSolutionEx(solution: soln1);
      _solutionExDatastore.Setup(x => x.BySolution(soln1.Id)).Returns(solnEx1);

      var search = new SearchDatastore(
        _dbConnectionFactory.Object,
        _logger.Object,
        _policy.Object,
        _frameworkDatastore.Object,
        _solutionDatastore.Object,
        _capabilityDatastore.Object,
        _claimedCapabilityDatastore.Object,
        _solutionExDatastore.Object);

      var results = search.ByKeyword(keyword);

      var res = results.Should().ContainSingle();
      res.Which.SolutionEx.Should().BeEquivalentTo(solnEx1);
      res.Which.Distance.Should().Be(1);
    }

    [TestCase("Does Really Kool document management", "doc")]
    [TestCase("Does Really Kool document management", "manage")]
    [TestCase("Does Really Kool patient collaboration", "collab")]
    [TestCase("Does Really Kool patient collaboration", "ient")]
    public void ByKeyword_KeywordMultiCapability_ReturnsSolution(
      string capabilityDescription,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln1 = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln1 });

      var cap1 = Creator.GetCapability(description: capabilityDescription);
      var cap2 = Creator.GetCapability(description: capabilityDescription);
      _capabilityDatastore.Setup(x => x.ById(cap1.Id)).Returns(cap1);
      _capabilityDatastore.Setup(x => x.ById(cap2.Id)).Returns(cap2);
      _capabilityDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { cap1, cap2 });

      var claimedCap11 = Creator.GetClaimedCapability(solutionId: soln1.Id, capabilityId: cap1.Id);
      _claimedCapabilityDatastore.Setup(x => x.BySolution(soln1.Id)).Returns(new[] { claimedCap11 });

      var solnEx1 = Creator.GetSolutionEx(solution: soln1);
      _solutionExDatastore.Setup(x => x.BySolution(soln1.Id)).Returns(solnEx1);

      var search = new SearchDatastore(
        _dbConnectionFactory.Object,
        _logger.Object,
        _policy.Object,
        _frameworkDatastore.Object,
        _solutionDatastore.Object,
        _capabilityDatastore.Object,
        _claimedCapabilityDatastore.Object,
        _solutionExDatastore.Object);

      var results = search.ByKeyword(keyword);

      var res = results.Should().ContainSingle();
      res.Which.SolutionEx.Should().BeEquivalentTo(solnEx1);
      res.Which.Distance.Should().Be(-1);
    }

    [TestCase("Does Really Kool document management", "doc")]
    [TestCase("Does Really Kool document management", "manage")]
    [TestCase("Does Really Kool patient collaboration", "collab")]
    [TestCase("Does Really Kool patient collaboration", "ient")]
    public void ByKeyword_MultiCapability_ReturnsSolution(
      string capabilityDescription,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln1 = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln1 });

      var cap1 = Creator.GetCapability(description: capabilityDescription);
      var cap2 = Creator.GetCapability(description: capabilityDescription);
      _capabilityDatastore.Setup(x => x.ById(cap1.Id)).Returns(cap1);
      _capabilityDatastore.Setup(x => x.ById(cap2.Id)).Returns(cap2);
      _capabilityDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { cap1, cap2 });

      var claimedCap11 = Creator.GetClaimedCapability(solutionId: soln1.Id, capabilityId: cap1.Id);
      var claimedCap12 = Creator.GetClaimedCapability(solutionId: soln1.Id, capabilityId: cap2.Id);
      _claimedCapabilityDatastore.Setup(x => x.BySolution(soln1.Id)).Returns(new[] { claimedCap11, claimedCap12 });

      var solnEx1 = Creator.GetSolutionEx(solution: soln1);
      _solutionExDatastore.Setup(x => x.BySolution(soln1.Id)).Returns(solnEx1);

      var search = new SearchDatastore(
        _dbConnectionFactory.Object,
        _logger.Object,
        _policy.Object,
        _frameworkDatastore.Object,
        _solutionDatastore.Object,
        _capabilityDatastore.Object,
        _claimedCapabilityDatastore.Object,
        _solutionExDatastore.Object);

      var results = search.ByKeyword(keyword);

      var res = results.Should().ContainSingle();
      res.Which.SolutionEx.Should().BeEquivalentTo(solnEx1);
      res.Which.Distance.Should().Be(0);
    }
  }
}
