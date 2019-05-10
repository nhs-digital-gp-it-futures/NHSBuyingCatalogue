using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Tests;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [TestFixture]
  public sealed class StandardsApplicableReviewsDatastore_Tests : DatastoreBase_Tests<StandardsApplicableReviewsDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new StandardsApplicableReviewsDatastore(DatastoreBaseSetup.StandardsApplicableReviewsDatastore, _logger, _policy));
    }

    [Test]
    public void CRUD_Succeeds()
    {
      var contact = Retriever.GetAllContacts(_policy).First();
      var orgDatastore = new OrganisationsDatastore(DatastoreBaseSetup.OrganisationsDatastore, new Mock<ILogger<OrganisationsDatastore>>().Object, _policy, _config, new Mock<ILongTermCache>().Object);
      var org = orgDatastore.ById(contact.OrganisationId);
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.SolutionsDatastore, new Mock<ILogger<SolutionsDatastore>>().Object, _policy, _config, new Mock<IShortTermCache>().Object, new Mock<IServiceProvider>().Object);
      var soln = solnDatastore.ByOrganisation(org.Id).First();
      var std = Retriever.GetAllStandards(_policy).First();
      var claimDatastore = new StandardsApplicableDatastore(DatastoreBaseSetup.StandardsApplicableDatastore, new Mock<ILogger<StandardsApplicableDatastore>>().Object, _policy);
      var evidenceDatastore = new StandardsApplicableEvidenceDatastore(DatastoreBaseSetup.StandardsApplicableEvidenceDatastore, new Mock<ILogger<StandardsApplicableEvidenceDatastore>>().Object, _policy);
      var datastore = new StandardsApplicableReviewsDatastore(DatastoreBaseSetup.StandardsApplicableReviewsDatastore, _logger, _policy);

      var newClaim = Creator.GetStandardsApplicable(solnId: soln.Id, claimId:std.Id, ownerId: contact.Id);
      var createdClaim = claimDatastore.Create(newClaim);
      StandardsApplicableReviews createdReview = null;

      try
      {
        var newEvidence = Creator.GetStandardsApplicableEvidence(claimId:createdClaim.Id, createdById: contact.Id);
        var createdEvidence = evidenceDatastore.Create(newEvidence);

        // create
        var newReview = Creator.GetStandardsApplicableReviews(evidenceId:createdEvidence.Id, createdById: contact.Id);
        createdReview = datastore.Create(newReview);

        createdReview.Should().BeEquivalentTo(newReview,
          opts => opts
            .Excluding(ent => ent.CreatedOn));

        // retrieve ById
        datastore.ById(createdReview.Id)
          .Should().NotBeNull()
          .And.Subject
          .Should().BeEquivalentTo(createdReview,
            opts => opts
              .Excluding(ent => ent.CreatedOn));

        // retrieve ByEvidence
        var retrievedReviews = datastore.ByEvidence(createdEvidence.Id)
          .SelectMany(x => x).ToList();
        retrievedReviews.Should().ContainSingle()
          .And.Subject.Single()
          .Should().BeEquivalentTo(createdReview,
            opts => opts
              .Excluding(ent => ent.CreatedOn));
      }
      finally
      {
        claimDatastore.Delete(createdClaim);
      }

      // delete
      datastore.ById(createdReview.Id)
        .Should().BeNull();
    }
  }
}
