using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [TestFixture]
  public sealed class CapabilitiesImplementedReviewsDatastore_Tests : DatastoreBase_Tests<CapabilitiesImplementedReviewsDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new CapabilitiesImplementedReviewsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void CRUD_Succeeds()
    {
      var contact = Retriever.GetAllContacts(_policy).First();
      var orgDatastore = new OrganisationsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<OrganisationsDatastore>>().Object, _policy);
      var org = orgDatastore.ById(contact.OrganisationId);
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var soln = solnDatastore.ByOrganisation(org.Id).First();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claimDatastore = new CapabilitiesImplementedDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<CapabilitiesImplementedDatastore>>().Object, _policy);
      var evidenceDatastore = new CapabilitiesImplementedEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<CapabilitiesImplementedEvidenceDatastore>>().Object, _policy);
      var datastore = new CapabilitiesImplementedReviewsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var newClaim = new CapabilitiesImplemented
      {
        Id = Guid.NewGuid().ToString(),
        SolutionId = soln.Id,
        CapabilityId = cap.Id,
        Status = CapabilitiesImplementedStatus.Draft
      };
      Verifier.Verify(newClaim);
      var createdClaim = claimDatastore.Create(newClaim);
      CapabilitiesImplementedReviews createdReview = null;

      try
      {
        var newEvidence = new CapabilitiesImplementedEvidence
        {
          Id = Guid.NewGuid().ToString(),
          ClaimId = createdClaim.Id,
          CreatedById = contact.Id,
          CreatedOn = DateTime.UtcNow
        };
        Verifier.Verify(newEvidence);
        var createdEvidence = evidenceDatastore.Create(newEvidence);

        // create
        var newReview = new CapabilitiesImplementedReviews
        {
          Id = Guid.NewGuid().ToString(),
          PreviousId = null,
          EvidenceId = createdEvidence.Id,
          CreatedById = contact.Id,
          CreatedOn = DateTime.UtcNow
        };
        Verifier.Verify(newReview);
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
