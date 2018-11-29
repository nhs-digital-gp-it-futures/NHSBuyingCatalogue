using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Datastore.CRM.Porcelain;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;
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

    private SolutionsExDatastore _datastore;
    private SolutionEx _solnOrig;
    private SolutionEx _solnEx;
    private Frameworks _framework;

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

      _datastore = new SolutionsExDatastore(
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

      var soln = Retriever.GetAllSolutions(_policy).First();
      _solnOrig = _datastore.BySolution(soln.Id);
      _solnEx = _solnOrig.Clone();

      var frameworksDatastore = new FrameworksDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<FrameworksDatastore>>().Object, _policy);
      _framework = frameworksDatastore.BySolution(_solnOrig.Solution.Id).ToList().Single();
    }

    [TearDown]
    public void TearDown()
    {
      _datastore.Update(_solnOrig);

      var linkMgr = new LinkManagerDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<LinkManagerDatastore>>().Object, _policy);
      linkMgr.FrameworkSolutionCreate(_framework.Id, _solnEx.Solution.Id);
    }

    [Test]
    public void Update_NoChange_Succeeds()
    {
      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx
        .Should().NotBeNull()
        .And.Subject
        .Should().BeEquivalentTo(_solnEx,
          opts => opts
            .Excluding(ent => ent.Solution.ModifiedOn)
            .Excluding(ent => ent.Solution.ModifiedById));
    }

    #region Solution
    [Test]
    public void Update_Solution_Succeeds()
    {
      _solnEx.Solution.Name = "My Solution for Everything and Then Some";
      _solnEx.Solution.Version = "This is the most current version";
      _solnEx.Solution.Description = "Insert 'War and Peace' here";
      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx
        .Should().NotBeNull()
        .And.Subject
        .Should().BeEquivalentTo(_solnEx,
          opts => opts
            .Excluding(ent => ent.Solution.CreatedOn)
            .Excluding(ent => ent.Solution.ModifiedOn)
            .Excluding(ent => ent.Solution.ModifiedById));
    }
    #endregion

    #region TechnicalContact
    [Test]
    public void Update_Add_TechnicalContact_Succeeds()
    {
      _solnEx.TechnicalContact.Clear();
      var techCont = Creator.GetTechnicalContact(solutionId: _solnEx.Solution.Id);
      _solnEx.TechnicalContact.Add(techCont);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.TechnicalContact
       .Should().ContainSingle()
       .And.Subject.Single()
       .Should().BeEquivalentTo(techCont);
    }

    [Test]
    public void Update_Remove_TechnicalContact_Succeeds()
    {
      _solnEx.TechnicalContact.Clear();
      _solnEx.TechnicalContact.Add(Creator.GetTechnicalContact(solutionId: _solnEx.Solution.Id));

      _datastore.Update(_solnEx);
      _solnEx.TechnicalContact.Clear();

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.TechnicalContact
        .Should().BeEmpty();
    }
    #endregion

    #region ClaimedCapability
    [Test]
    public void Update_Add_ClaimedCapability_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapability
        .Should().ContainSingle()
        .And.Subject
        .Should().BeEquivalentTo(claim);
    }

    [Test]
    public void Update_Remove_ClaimedCapability_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      _datastore.Update(_solnEx);
      _solnEx.ClaimedCapability.Clear();

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapability
        .Should().BeEmpty();
    }
    #endregion

    #region ClaimedStandard
    [Test]
    public void Update_Add_ClaimedStandard_Succeeds()
    {
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedStandard();
      _solnEx.ClaimedStandard.Add(claim);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedStandard
        .Should().ContainSingle()
        .And
        .Should().BeEquivalentTo(claim);
    }

    [Test]
    public void Update_Remove_ClaimedStandard_Succeeds()
    {
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedStandard();
      _solnEx.ClaimedStandard.Add(claim);

      _datastore.Update(_solnEx);
      _solnEx.ClaimedStandard.Clear();

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedStandard
        .Should().BeEmpty();
    }
    #endregion

    #region ClaimedCapabilityEvidence
    [Test]
    public void Update_Add_ClaimedCapabilityEvidence_NoChain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      _datastore.Update(_solnEx);
      var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      _solnEx.ClaimedCapabilityEvidence.Clear();
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityEvidence
        .Should().ContainSingle()
        .And
        .Should().BeEquivalentTo(evidence,
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }

    [Test]
    public void Update_Add_ClaimedCapabilityEvidence_Chain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      _datastore.Update(_solnEx);
      var evidencePrev = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      var evidence = Creator.GetCapabilitiesImplementedEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
      _solnEx.ClaimedCapabilityEvidence.Clear();
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _solnEx.ClaimedCapabilityEvidence.Add(evidencePrev);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityEvidence
        .Should().HaveCount(2)
        .And.Subject
        .Should().BeEquivalentTo(new[] { evidence, evidencePrev },
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }

    [Test]
    public void Update_Add_ClaimedCapabilityEvidence_EndChain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidencePrev = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidencePrev);
      _datastore.Update(_solnEx);
      var evidence = Creator.GetCapabilitiesImplementedEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityEvidence
        .Should().HaveCount(2)
        .And.Subject
        .Should().BeEquivalentTo(new[] { evidence, evidencePrev },
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }

    [Test]
    public void Update_Remove_ClaimedCapabilityEvidence_NoChain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _datastore.Update(_solnEx);
      _solnEx.ClaimedCapabilityEvidence.Clear();

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityEvidence
        .Should().BeEmpty();
    }

    [Test]
    public void Update_Remove_ClaimedCapabilityEvidence_Chain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidencePrev = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      var evidence = Creator.GetCapabilitiesImplementedEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _solnEx.ClaimedCapabilityEvidence.Add(evidencePrev);
      _datastore.Update(_solnEx);
      _solnEx.ClaimedCapabilityEvidence.Clear();

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityEvidence
        .Should().BeEmpty();
    }

    [Test]
    public void Update_Remove_ClaimedCapabilityEvidence_EndChain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidencePrev = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      var evidence = Creator.GetCapabilitiesImplementedEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _solnEx.ClaimedCapabilityEvidence.Add(evidencePrev);
      _datastore.Update(_solnEx);
      _solnEx.ClaimedCapabilityEvidence.Remove(evidence);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityEvidence
        .Should().ContainSingle()
        .And
        .Should().BeEquivalentTo(evidencePrev,
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }
    #endregion

    #region ClaimedStandardEvidence
    [Test]
    public void Update_Add_ClaimedStandardEvidence_NoChain_Succeeds()
    {
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedStandard();
      _solnEx.ClaimedStandard.Add(claim);

      _datastore.Update(_solnEx);
      var evidence = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
      _solnEx.ClaimedStandardEvidence.Add(evidence);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedStandardEvidence
        .Should().ContainSingle()
        .And
        .Should().BeEquivalentTo(evidence,
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }

    [Test]
    public void Update_Add_ClaimedStandardEvidence_Chain_Succeeds()
    {
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedStandard();
      _solnEx.ClaimedStandard.Add(claim);

      _datastore.Update(_solnEx);
      var evidencePrev = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
      var evidence = Creator.GetStandardsApplicableEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
      _solnEx.ClaimedStandardEvidence.Add(evidence);
      _solnEx.ClaimedStandardEvidence.Add(evidencePrev);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedStandardEvidence
        .Should().HaveCount(2)
        .And.Subject
        .Should().BeEquivalentTo(new[] { evidence, evidencePrev },
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }

    [Test]
    public void Update_Add_ClaimedStandardEvidence_EndChain_Succeeds()
    {
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedStandard();
      _solnEx.ClaimedStandard.Add(claim);

      var evidencePrev = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
      _solnEx.ClaimedStandardEvidence.Add(evidencePrev);
      _datastore.Update(_solnEx);
      var evidence = Creator.GetStandardsApplicableEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
      _solnEx.ClaimedStandardEvidence.Add(evidence);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedStandardEvidence
        .Should().HaveCount(2)
        .And.Subject
        .Should().BeEquivalentTo(new[] { evidence, evidencePrev },
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }

    [Test]
    public void Update_Remove_ClaimedStandardEvidence_NoChain_Succeeds()
    {
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedStandard();
      _solnEx.ClaimedStandard.Add(claim);

      var evidence = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
      _solnEx.ClaimedStandardEvidence.Add(evidence);
      _datastore.Update(_solnEx);
      _solnEx.ClaimedStandardEvidence.Clear();

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedStandardEvidence
        .Should().BeEmpty();
    }

    [Test]
    public void Update_Remove_ClaimedStandardEvidence_Chain_Succeeds()
    {
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedStandard();
      _solnEx.ClaimedStandard.Add(claim);

      var evidencePrev = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
      var evidence = Creator.GetStandardsApplicableEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
      _solnEx.ClaimedStandardEvidence.Add(evidence);
      _solnEx.ClaimedStandardEvidence.Add(evidencePrev);
      _datastore.Update(_solnEx);
      _solnEx.ClaimedStandardEvidence.Clear();

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedStandardEvidence
        .Should().BeEmpty();
    }

    [Test]
    public void Update_Remove_ClaimedStandardEvidence_EndChain_Succeeds()
    {
      var std = Retriever.GetAllStandards(_policy).First();
      var claim = Creator.GetStandardsApplicable(claimId: std.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedStandard();
      _solnEx.ClaimedStandard.Add(claim);

      var evidencePrev = Creator.GetStandardsApplicableEvidence(claimId: claim.Id);
      var evidence = Creator.GetStandardsApplicableEvidence(prevId: evidencePrev.Id, claimId: claim.Id);
      _solnEx.ClaimedStandardEvidence.Add(evidence);
      _solnEx.ClaimedStandardEvidence.Add(evidencePrev);
      _datastore.Update(_solnEx);
      _solnEx.ClaimedStandardEvidence.Remove(evidence);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedStandardEvidence
        .Should().ContainSingle()
        .And
        .Should().BeEquivalentTo(evidencePrev,
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }
    #endregion

    #region ClaimedCapabilityReview
    [Test]
    public void Update_Add_ClaimedCapabilityReview_NoChain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _datastore.Update(_solnEx);
      var review = Creator.GetCapabilitiesImplementedReviews(evidenceId: evidence.Id);
      _solnEx.ClaimedCapabilityReview.Add(review);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityReview
        .Should().ContainSingle()
        .And
        .Should().BeEquivalentTo(review,
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }

    [Test]
    public void Update_Add_ClaimedCapabilityReview_Chain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _datastore.Update(_solnEx);
      var reviewPrev = Creator.GetCapabilitiesImplementedReviews(evidenceId: evidence.Id);
      var review = Creator.GetCapabilitiesImplementedReviews(prevId: reviewPrev.Id, evidenceId: evidence.Id);
      _solnEx.ClaimedCapabilityReview.Add(reviewPrev);
      _solnEx.ClaimedCapabilityReview.Add(review);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityReview
        .Should().HaveCount(2)
        .And.Subject
        .Should().BeEquivalentTo(new[] { review, reviewPrev },
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }

    [Test]
    public void Update_Add_ClaimedCapabilityReview_EndChain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      var reviewPrev = Creator.GetCapabilitiesImplementedReviews(evidenceId: evidence.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _solnEx.ClaimedCapabilityReview.Add(reviewPrev);
      _datastore.Update(_solnEx);
      var review = Creator.GetCapabilitiesImplementedReviews(prevId: reviewPrev.Id, evidenceId: evidence.Id);
      _solnEx.ClaimedCapabilityReview.Add(review);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityReview
        .Should().HaveCount(2)
        .And.Subject
        .Should().BeEquivalentTo(new[] { review, reviewPrev },
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }

    [Test]
    public void Update_Remove_ClaimedCapabilityReview_NoChain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      var review = Creator.GetCapabilitiesImplementedReviews(evidenceId: evidence.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _solnEx.ClaimedCapabilityReview.Add(review);
      _datastore.Update(_solnEx);
      _solnEx.ClaimedCapabilityReview.Clear();

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityReview
        .Should().BeEmpty();
    }

    [Test]
    public void Update_Remove_ClaimedCapabilityReview_Chain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      var reviewPrev = Creator.GetCapabilitiesImplementedReviews(evidenceId: evidence.Id);
      var review = Creator.GetCapabilitiesImplementedReviews(prevId: reviewPrev.Id, evidenceId: evidence.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _solnEx.ClaimedCapabilityReview.Add(reviewPrev);
      _solnEx.ClaimedCapabilityReview.Add(review);
      _datastore.Update(_solnEx);
      _solnEx.ClaimedCapabilityReview.Clear();

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityReview
        .Should().BeEmpty();
    }

    [Test]
    public void Update_Remove_ClaimedCapabilityReview_EndChain_Succeeds()
    {
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claim = Creator.GetCapabilitiesImplemented(claimId: cap.Id, solnId: _solnEx.Solution.Id);
      ClearClaimedCapability();
      _solnEx.ClaimedCapability.Add(claim);

      var evidence = Creator.GetCapabilitiesImplementedEvidence(claimId: claim.Id);
      var reviewPrev = Creator.GetCapabilitiesImplementedReviews(evidenceId: evidence.Id);
      var review = Creator.GetCapabilitiesImplementedReviews(prevId: reviewPrev.Id, evidenceId: evidence.Id);
      _solnEx.ClaimedCapabilityEvidence.Add(evidence);
      _solnEx.ClaimedCapabilityReview.Add(reviewPrev);
      _solnEx.ClaimedCapabilityReview.Add(review);
      _datastore.Update(_solnEx);
      _solnEx.ClaimedCapabilityReview.Remove(review);

      _datastore.Update(_solnEx);

      var retrievedSolnEx = _datastore.BySolution(_solnEx.Solution.Id);
      retrievedSolnEx.ClaimedCapabilityReview
        .Should().ContainSingle()
        .And
        .Should().BeEquivalentTo(reviewPrev,
          opts => opts
            .Excluding(ent => ent.CreatedOn));
    }
    #endregion

    private void ClearClaimedCapability()
    {
      _solnEx.ClaimedCapability.Clear();
      _solnEx.ClaimedCapabilityEvidence.Clear();
      _solnEx.ClaimedCapabilityReview.Clear();
    }

    private void ClearClaimedStandard()
    {
      _solnEx.ClaimedStandard.Clear();
      _solnEx.ClaimedStandardEvidence.Clear();
      _solnEx.ClaimedStandardReview.Clear();
    }
  }
}
