using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using NUnit.Framework;
using System.Collections.Generic;
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
      var datas = GetAll();

      datas.Should().NotBeEmpty();
      datas.ForEach(data => data.Should().NotBeNull());
      datas.ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void Update_NoChange_Succeeds()
    {
      var solnEx = GetAll().First();
      var datastore = GetDatastore();

      datastore.Update(solnEx);

      var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
      retrievedSolnEx
        .Should().NotBeNull()
        .And.Subject
        .Should().BeEquivalentTo(solnEx,
          opts => opts
            .Excluding(ent => ent.Solution.ModifiedOn)
            .Excluding(ent => ent.Solution.ModifiedById));
    }

    [Test]
    public void Update_AddTechnicalContact_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      solnEx.TechnicalContact.Add(Creator.GetTechnicalContact(solutionId: solnEx.Solution.Id));

      try
      {
        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx
          .Should().NotBeNull()
          .And.Subject
          .Should().BeEquivalentTo(solnEx,
            opts => opts
              .Excluding(ent => ent.Solution.ModifiedOn)
              .Excluding(ent => ent.Solution.ModifiedById));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_RemoveTechnicalContact_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      solnEx.TechnicalContact.Add(Creator.GetTechnicalContact(solutionId: solnEx.Solution.Id));

      try
      {
        datastore.Update(solnEx);
        solnEx.TechnicalContact.Clear();

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx
          .Should().NotBeNull()
          .And.Subject
          .Should().BeEquivalentTo(solnEx,
            opts => opts
              .Excluding(ent => ent.Solution.ModifiedOn)
              .Excluding(ent => ent.Solution.ModifiedById));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_AddClaimedCapability_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var claim = Retriever.GetAllCapabilities(_policy).First();
      solnEx.ClaimedCapability.Add(Creator.GetCapabilitiesImplemented(claimId: claim.Id, solnId: solnEx.Solution.Id));

      try
      {
        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx
          .Should().NotBeNull()
          .And.Subject
          .Should().BeEquivalentTo(solnEx,
            opts => opts
              .Excluding(ent => ent.Solution.ModifiedOn)
              .Excluding(ent => ent.Solution.ModifiedById));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_RemoveClaimedCapability_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var claim = Retriever.GetAllCapabilities(_policy).First();
      solnEx.ClaimedCapability.Add(Creator.GetCapabilitiesImplemented(claimId: claim.Id, solnId: solnEx.Solution.Id));

      try
      {
        datastore.Update(solnEx);
        solnEx.ClaimedCapability.Clear();

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx
          .Should().NotBeNull()
          .And.Subject
          .Should().BeEquivalentTo(solnEx,
            opts => opts
              .Excluding(ent => ent.Solution.ModifiedOn)
              .Excluding(ent => ent.Solution.ModifiedById));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_AddStandardsApplicable_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var claim = Retriever.GetAllStandards(_policy).First();
      solnEx.ClaimedStandard.Add(Creator.GetStandardsApplicable(claimId: claim.Id, solnId: solnEx.Solution.Id));

      try
      {
        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx
          .Should().NotBeNull()
          .And.Subject
          .Should().BeEquivalentTo(solnEx,
            opts => opts
              .Excluding(ent => ent.Solution.ModifiedOn)
              .Excluding(ent => ent.Solution.ModifiedById));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_RemoveStandardsApplicable_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var claim = Retriever.GetAllStandards(_policy).First();
      solnEx.ClaimedStandard.Add(Creator.GetStandardsApplicable(claimId: claim.Id, solnId: solnEx.Solution.Id));

      try
      {
        datastore.Update(solnEx);
        solnEx.ClaimedStandard.Clear();

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx
          .Should().NotBeNull()
          .And.Subject
          .Should().BeEquivalentTo(solnEx,
            opts => opts
              .Excluding(ent => ent.Solution.ModifiedOn)
              .Excluding(ent => ent.Solution.ModifiedById));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    private List<SolutionEx> GetAll()
    {
      var allSolns = Retriever.GetAllSolutions(_policy);
      var datastore = GetDatastore();
      var datas = allSolns.Select(soln => datastore.BySolution(soln.Id)).ToList();

      return datas;
    }

    private SolutionsExDatastore GetDatastore()
    {
      return new SolutionsExDatastore(
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
    }
  }
}
