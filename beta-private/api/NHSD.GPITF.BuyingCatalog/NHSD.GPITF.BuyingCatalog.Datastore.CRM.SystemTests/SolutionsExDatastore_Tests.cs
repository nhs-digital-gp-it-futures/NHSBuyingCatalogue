using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain;
using NHSD.GPITF.BuyingCatalog.Logic;
using NUnit.Framework;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [TestFixture]
  public sealed class SolutionsExDatastore_Tests : DatastoreBase_Tests<SolutionsExDatastore>
  {
    private SolutionsDatastore _solutionDatastore;
    private TechnicalContactsDatastore _technicalContactDatastore;

    private CapabilitiesImplementedDatastore _claimedCapabilityDatastore;
    private CapabilitiesImplementedEvidenceDatastore _claimedCapabilityEvidenceDatastore;
    private CapabilitiesImplementedReviewsDatastore _claimedCapabilityReviewsDatastore;

    private StandardsApplicableDatastore _claimedStandardDatastore;
    private StandardsApplicableEvidenceDatastore _claimedStandardEvidenceDatastore;
    private StandardsApplicableReviewsDatastore _claimedStandardReviewsDatastore;

    [SetUp]
    public void Setup()
    {
      _solutionDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      _technicalContactDatastore = new TechnicalContactsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<TechnicalContactsDatastore>>().Object, _policy);

      _claimedCapabilityDatastore = new CapabilitiesImplementedDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<CapabilitiesImplementedDatastore>>().Object, _policy);
      _claimedCapabilityEvidenceDatastore = new CapabilitiesImplementedEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<CapabilitiesImplementedEvidenceDatastore>>().Object, _policy);
      _claimedCapabilityReviewsDatastore = new CapabilitiesImplementedReviewsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<CapabilitiesImplementedReviewsDatastore>>().Object, _policy);

      _claimedStandardDatastore = new StandardsApplicableDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<StandardsApplicableDatastore>>().Object, _policy);
      _claimedStandardEvidenceDatastore = new StandardsApplicableEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<StandardsApplicableEvidenceDatastore>>().Object, _policy);
      _claimedStandardReviewsDatastore = new StandardsApplicableReviewsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<StandardsApplicableReviewsDatastore>>().Object, _policy);
    }

    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new SolutionsExDatastore(
        DatastoreBaseSetup.CrmConnectionFactory,
        _logger,
        _policy,

        _solutionDatastore,
        _technicalContactDatastore,

        _claimedCapabilityDatastore,
        _claimedCapabilityEvidenceDatastore,
        _claimedCapabilityReviewsDatastore,

        _claimedStandardDatastore,
        _claimedStandardEvidenceDatastore,
        _claimedStandardReviewsDatastore
        ));
    }

    [Test]
    public void BySolution_ReturnsData()
    {
      var allSolns = Retriever.GetAllSolutions(_policy);
      var datastore = new SolutionsExDatastore(
        DatastoreBaseSetup.CrmConnectionFactory,
        _logger,
        _policy,

        _solutionDatastore,
        _technicalContactDatastore,

        _claimedCapabilityDatastore,
        _claimedCapabilityEvidenceDatastore,
        _claimedCapabilityReviewsDatastore,

        _claimedStandardDatastore,
        _claimedStandardEvidenceDatastore,
        _claimedStandardReviewsDatastore);

      var datas = allSolns.Select(soln => datastore.BySolution(soln.Id)).ToList();

      datas.Should().NotBeEmpty();
      datas.ForEach(data => data.Should().NotBeNull());
      datas.ForEach(data => Verifier.Verify(data));
    }
  }
}
