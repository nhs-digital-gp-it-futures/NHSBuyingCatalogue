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

    #region TechnicalContact
    [Test]
    public void Update_Add_TechnicalContact_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();

      try
      {
        solnEx.TechnicalContact.Clear();
        var techCont = Creator.GetTechnicalContact(solutionId: solnEx.Solution.Id);
        solnEx.TechnicalContact.Add(techCont);
        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.TechnicalContact
         .Should().ContainSingle()
         .And
         .Should().BeEquivalentTo(techCont);
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Remove_TechnicalContact_Succeeds()
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
        retrievedSolnEx.TechnicalContact
          .Should().BeEmpty();
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }
    #endregion

    #region ClaimedCapability
    [Test]
    public void Update_Add_ClaimedCapability_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedCapability.Add(claim);

      try
      {
        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedCapability
          .Should().ContainSingle()
          .And
          .Should().BeEquivalentTo(claim);
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Remove_ClaimedCapability_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedCapability.Add(claim);

      try
      {
        datastore.Update(solnEx);
        solnEx.ClaimedCapability.Clear();

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedCapability
          .Should().BeEmpty();
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }
    #endregion

    #region ClaimedStandard
    [Test]
    public void Update_Add_ClaimedStandard_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedStandard.Add(claim);

      try
      {
        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedStandard
          .Should().ContainSingle()
          .And
          .Should().BeEquivalentTo(claim);
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Remove_ClaimedStandard_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedStandard.Add(claim);

      try
      {
        datastore.Update(solnEx);
        solnEx.ClaimedStandard.Clear();

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedStandard
          .Should().BeEmpty();
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }
    #endregion

    #region ClaimedCapabilityEvidence
    [Test]
    public void Update_Add_ClaimedCapabilityEvidence_NoChain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedCapability.Add(claim);

      try
      {
        datastore.Update(solnEx);
        var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
        solnEx.ClaimedCapabilityEvidence.Add(evidence);

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedCapabilityEvidence
          .Should().ContainSingle()
          .And
          .Should().BeEquivalentTo(evidence,
            opts => opts
              .Excluding(ent => ent.CreatedOn));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Add_ClaimedCapabilityEvidence_Chain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedCapability.Add(claim);

      try
      {
        datastore.Update(solnEx);
        var evidencePrev = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
        var evidence = Creator.GetCapabilitiesImplementedEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
        solnEx.ClaimedCapabilityEvidence.Add(evidence);
        solnEx.ClaimedCapabilityEvidence.Add(evidencePrev);

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedCapabilityEvidence
          .Should().HaveCount(2)
          .And.Subject
          .Should().BeEquivalentTo(new[] { evidence, evidencePrev },
            opts => opts
              .Excluding(ent => ent.CreatedOn));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Add_ClaimedCapabilityEvidence_EndChain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedCapability.Add(claim);

      try
      {
        var evidencePrev = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
        solnEx.ClaimedCapabilityEvidence.Add(evidencePrev);
        datastore.Update(solnEx);
        var evidence = Creator.GetCapabilitiesImplementedEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
        solnEx.ClaimedCapabilityEvidence.Add(evidence);

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedCapabilityEvidence
          .Should().HaveCount(2)
          .And.Subject
          .Should().BeEquivalentTo(new[] { evidence, evidencePrev },
            opts => opts
              .Excluding(ent => ent.CreatedOn));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Remove_ClaimedCapabilityEvidence_NoChain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedCapability.Add(claim);

      try
      {
        var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
        solnEx.ClaimedCapabilityEvidence.Add(evidence);
        datastore.Update(solnEx);
        solnEx.ClaimedCapabilityEvidence.Clear();

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedCapabilityEvidence
          .Should().BeEmpty();
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Remove_ClaimedCapabilityEvidence_Chain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedCapability.Add(claim);

      try
      {
        var evidencePrev = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
        var evidence = Creator.GetCapabilitiesImplementedEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
        solnEx.ClaimedCapabilityEvidence.Add(evidence);
        solnEx.ClaimedCapabilityEvidence.Add(evidencePrev);
        datastore.Update(solnEx);
        solnEx.ClaimedCapabilityEvidence.Clear();

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedCapabilityEvidence
          .Should().BeEmpty();
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Remove_ClaimedCapabilityEvidence_EndChain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedCapability.Add(claim);

      try
      {
        var evidencePrev = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
        var evidence = Creator.GetCapabilitiesImplementedEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
        solnEx.ClaimedCapabilityEvidence.Add(evidence);
        solnEx.ClaimedCapabilityEvidence.Add(evidencePrev);
        datastore.Update(solnEx);
        solnEx.ClaimedCapabilityEvidence.Remove(evidence);

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedCapabilityEvidence
          .Should().ContainSingle()
          .And
          .Should().BeEquivalentTo(evidence,
            opts => opts
              .Excluding(ent => ent.CreatedOn));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }
    #endregion

    #region ClaimedStandardEvidence
    [Test]
    public void Update_Add_ClaimedStandardEvidence_NoChain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedStandard.Add(claim);

      try
      {
        datastore.Update(solnEx);
        var evidence = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
        solnEx.ClaimedStandardEvidence.Add(evidence);

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedStandardEvidence
          .Should().ContainSingle()
          .And
          .Should().BeEquivalentTo(evidence,
            opts => opts
              .Excluding(ent => ent.CreatedOn));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Add_ClaimedStandardEvidence_Chain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedStandard.Add(claim);

      try
      {
        datastore.Update(solnEx);
        var evidencePrev = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
        var evidence = Creator.GetStandardsApplicableEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
        solnEx.ClaimedStandardEvidence.Add(evidence);
        solnEx.ClaimedStandardEvidence.Add(evidencePrev);

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedStandardEvidence
          .Should().HaveCount(2)
          .And.Subject
          .Should().BeEquivalentTo(new[] { evidence, evidencePrev },
            opts => opts
              .Excluding(ent => ent.CreatedOn));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Add_ClaimedStandardEvidence_EndChain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedStandard.Add(claim);

      try
      {
        var evidencePrev = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
        solnEx.ClaimedStandardEvidence.Add(evidencePrev);
        datastore.Update(solnEx);
        var evidence = Creator.GetStandardsApplicableEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
        solnEx.ClaimedStandardEvidence.Add(evidence);

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedStandardEvidence
          .Should().HaveCount(2)
          .And.Subject
          .Should().BeEquivalentTo(new[] { evidence, evidencePrev },
            opts => opts
              .Excluding(ent => ent.CreatedOn));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Remove_ClaimedStandardEvidence_NoChain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedStandard.Add(claim);

      try
      {
        var evidence = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
        solnEx.ClaimedStandardEvidence.Add(evidence);
        datastore.Update(solnEx);
        solnEx.ClaimedStandardEvidence.Clear();

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedStandardEvidence
          .Should().BeEmpty();
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Remove_ClaimedStandardEvidence_Chain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedStandard.Add(claim);

      try
      {
        var evidencePrev = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
        var evidence = Creator.GetStandardsApplicableEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
        solnEx.ClaimedStandardEvidence.Add(evidence);
        solnEx.ClaimedStandardEvidence.Add(evidencePrev);
        datastore.Update(solnEx);
        solnEx.ClaimedStandardEvidence.Clear();

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedStandardEvidence
          .Should().BeEmpty();
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }

    [Test]
    public void Update_Remove_ClaimedStandardEvidence_EndChain_Succeeds()
    {
      var allSolnEx = GetAll();
      var solnOrig = allSolnEx.First();
      var solnEx = allSolnEx.First();
      solnOrig.Should().BeEquivalentTo(solnEx);
      var datastore = GetDatastore();
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: solnEx.Solution.Id);
      solnEx.ClaimedStandard.Add(claim);

      try
      {
        var evidencePrev = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
        var evidence = Creator.GetStandardsApplicableEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
        solnEx.ClaimedStandardEvidence.Add(evidence);
        solnEx.ClaimedStandardEvidence.Add(evidencePrev);
        datastore.Update(solnEx);
        solnEx.ClaimedStandardEvidence.Remove(evidence);

        datastore.Update(solnEx);

        var retrievedSolnEx = datastore.BySolution(solnEx.Solution.Id);
        retrievedSolnEx.ClaimedStandardEvidence
          .Should().ContainSingle()
          .And
          .Should().BeEquivalentTo(evidencePrev,
            opts => opts
              .Excluding(ent => ent.CreatedOn));
      }
      finally
      {
        datastore.Update(solnOrig);
      }
    }
    #endregion

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
