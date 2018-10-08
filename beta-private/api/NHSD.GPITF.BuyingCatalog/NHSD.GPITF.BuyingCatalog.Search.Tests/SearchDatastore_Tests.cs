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
    public void SolutionExByKeyword_KeywordInSolutionName_ReturnsSolution(
      string solutionName,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution(name: solutionName);
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability();
      _capabilityDatastore.Setup(x => x.GetAll()).Returns(new[] { capability });

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

      var results = search.SolutionExByKeyword(keyword);

      var resSoln = results.Should().ContainSingle();
      resSoln.Subject.Solution.Should().BeEquivalentTo(soln);
    }

    [TestCase("Does Really Kool document management", "doc")]
    [TestCase("Does Really Kool document management", "manage")]
    [TestCase("Does Really Kool patient collaboration", "collab")]
    [TestCase("Does Really Kool patient collaboration", "ient")]
    public void SolutionExByKeyword_KeywordInSolutionDescription_ReturnsSolution(
      string solutionDescription,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution(description: solutionDescription);
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability();
      _capabilityDatastore.Setup(x => x.GetAll()).Returns(new[] { capability });

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

      var results = search.SolutionExByKeyword(keyword);

      var resSoln = results.Should().ContainSingle();
      resSoln.Subject.Solution.Should().BeEquivalentTo(soln);
    }

    [TestCase("Document Manager", "docs")]
    [TestCase("Document Manager", "manages")]
    [TestCase("Patient Collaboration", "collaborates")]
    [TestCase("Patient Collaboration", "sentient")]
    public void SolutionExByKeyword_KeywordNotInSolutionName_ReturnsNone(
      string solutionName,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution(name: solutionName);
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability();
      _capabilityDatastore.Setup(x => x.GetAll()).Returns(new[] { capability });

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

      var results = search.SolutionExByKeyword(keyword);

      results.Should().BeEmpty();
    }

    [TestCase("Does Really Kool document management", "docs")]
    [TestCase("Does Really Kool document management", "manages")]
    [TestCase("Does Really Kool patient collaboration", "collaborates")]
    [TestCase("Does Really Kool patient collaboration", "sentient")]
    public void SolutionExByKeyword_KeywordNotInSolutionDescription_ReturnsNone(
      string solutionDescription,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution(description: solutionDescription);
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability();
      _capabilityDatastore.Setup(x => x.GetAll()).Returns(new[] { capability });

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

      var results = search.SolutionExByKeyword(keyword);

      results.Should().BeEmpty();
    }

    [TestCase("Document Manager", "doc")]
    [TestCase("Document Manager", "manage")]
    [TestCase("Patient Collaboration", "collab")]
    [TestCase("Patient Collaboration", "ient")]
    public void SolutionExByKeyword_KeywordInCapabilityName_ReturnsSolution(
      string capabilityName,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability(name: capabilityName);
      _capabilityDatastore.Setup(x => x.GetAll()).Returns(new[] { capability });

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

      var results = search.SolutionExByKeyword(keyword);

      var resSoln = results.Should().ContainSingle();
      resSoln.Subject.Solution.Should().BeEquivalentTo(soln);
    }

    [TestCase("Does Really Kool document management", "doc")]
    [TestCase("Does Really Kool document management", "manage")]
    [TestCase("Does Really Kool patient collaboration", "collab")]
    [TestCase("Does Really Kool patient collaboration", "ient")]
    public void SolutionExByKeyword_KeywordInCapabilityDescription_ReturnsSolution(
      string capabilityDescription,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability(description: capabilityDescription);
      _capabilityDatastore.Setup(x => x.GetAll()).Returns(new[] { capability });

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

      var results = search.SolutionExByKeyword(keyword);

      var resSoln = results.Should().ContainSingle();
      resSoln.Subject.Solution.Should().BeEquivalentTo(soln);
    }

    [TestCase("Document Manager", "docs")]
    [TestCase("Document Manager", "manages")]
    [TestCase("Patient Collaboration", "collaborates")]
    [TestCase("Patient Collaboration", "sentient")]
    public void SolutionExByKeyword_KeywordNotInCapabilityName_ReturnsNone(
      string capabilityName,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability(name: capabilityName);
      _capabilityDatastore.Setup(x => x.GetAll()).Returns(new[] { capability });

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

      var results = search.SolutionExByKeyword(keyword);

      results.Should().BeEmpty();
    }

    [TestCase("Does Really Kool document management", "docs")]
    [TestCase("Does Really Kool document management", "manages")]
    [TestCase("Does Really Kool patient collaboration", "collaborates")]
    [TestCase("Does Really Kool patient collaboration", "sentient")]
    public void SolutionExByKeyword_KeywordNotInCapabilityDescription_ReturnsNone(
      string capabilityDescription,
      string keyword)
    {
      var framework = Creator.GetFramework();
      _frameworkDatastore.Setup(x => x.GetAll()).Returns(new[] { framework });

      var soln = Creator.GetSolution();
      _solutionDatastore.Setup(x => x.ByFramework(framework.Id)).Returns(new[] { soln });

      var capability = Creator.GetCapability(description: capabilityDescription);
      _capabilityDatastore.Setup(x => x.GetAll()).Returns(new[] { capability });

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

      var results = search.SolutionExByKeyword(keyword);

      results.Should().BeEmpty();
    }
  }
}
