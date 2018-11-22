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
  public sealed class StandardsApplicableReviewsDatastore_Tests : DatastoreBase_Tests<StandardsApplicableReviewsDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new StandardsApplicableReviewsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void CRUD_Succeeds()
    {
      var contact = Retriever.GetAllContacts(_policy).First();
      var orgDatastore = new OrganisationsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<OrganisationsDatastore>>().Object, _policy);
      var org = orgDatastore.ById(contact.OrganisationId);
      var solnDatastore = new SolutionsDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<SolutionsDatastore>>().Object, _policy);
      var soln = solnDatastore.ByOrganisation(org.Id).First();
      var std = Retriever.GetAllStandards(_policy).First();
      var claimDatastore = new StandardsApplicableDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<StandardsApplicableDatastore>>().Object, _policy);
      var evidenceDatastore = new StandardsApplicableEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<StandardsApplicableEvidenceDatastore>>().Object, _policy);
      var datastore = new StandardsApplicableReviewsDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var newClaim = new StandardsApplicable
      {
        Id = Guid.NewGuid().ToString(),
        SolutionId = soln.Id,
        StandardId = std.Id,
        Status = StandardsApplicableStatus.Draft
      };
      Verifier.Verify(newClaim);
      var createdClaim = claimDatastore.Create(newClaim);
      StandardsApplicableReviews createdReview = null;

      try
      {
        var newEvidence = new StandardsApplicableEvidence
        {
          Id = Guid.NewGuid().ToString(),
          ClaimId = createdClaim.Id,
          CreatedById = contact.Id,
          CreatedOn = DateTime.UtcNow
        };
        Verifier.Verify(newEvidence);
        var createdEvidence = evidenceDatastore.Create(newEvidence);

        // create
        var newReview = new StandardsApplicableReviews
        {
          Id = Guid.NewGuid().ToString(),
          PreviousId = null,
          EvidenceId = createdEvidence.Id,
          CreatedById = contact.Id,
          CreatedOn = DateTime.UtcNow
        };
        Verifier.Verify(newReview);
        createdReview = datastore.Create(newReview);

        createdReview.Should().BeEquivalentTo(newReview);

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
