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
    public void Crud_Succeeds()
    {
      var soln = Retriever.GetAllSolutions(_policy).First();
      var contact = Retriever.GetAllContacts(_policy).First(cont => cont.OrganisationId == soln.OrganisationId);
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
      var createdReview = datastore.Create(newReview);
      createdReview.Should().BeEquivalentTo(newReview, opt => opt.Excluding(rev => rev.Id));

      try
      {
        // retrieve ById
        datastore.ById(createdReview.Id)
          .Should().NotBeNull()
          .And
          .Should().BeEquivalentTo(createdReview);

        // retrieve ByEvidence
        datastore.ByEvidence(createdEvidence.Id)
          .Should().ContainSingle()
          .And
          .Should().BeEquivalentTo(createdReview);
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
