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
  public sealed class StandardsApplicableEvidenceDatastore_Tests : DatastoreBase_Tests<StandardsApplicableEvidenceDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new StandardsApplicableEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void CRUD_Succeeds()
    {
      var soln = Retriever.GetAllSolutions(_policy).First();
      var contact = Retriever.GetAllContacts(_policy).First(cont => cont.OrganisationId == soln.OrganisationId);
      var std = Retriever.GetAllStandards(_policy).First();
      var claimDatastore = new StandardsApplicableDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<StandardsApplicableDatastore>>().Object, _policy);
      var datastore = new StandardsApplicableEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var newClaim = new StandardsApplicable
      {
        Id = Guid.NewGuid().ToString(),
        SolutionId = soln.Id,
        StandardId = std.Id,
        Status = StandardsApplicableStatus.Draft
      };
      Verifier.Verify(newClaim);
      var createdClaim = claimDatastore.Create(newClaim);

      // create
      var newEvidence = new StandardsApplicableEvidence
      {
        Id = Guid.NewGuid().ToString(),
        ClaimId = createdClaim.Id,
        CreatedById = contact.Id,
        CreatedOn = DateTime.UtcNow
      };
      Verifier.Verify(newEvidence);
      var createdEvidence = datastore.Create(newEvidence);
      createdEvidence.Should().BeEquivalentTo(newEvidence);

      try
      {
        // retrieve ById
        datastore.ById(createdEvidence.Id)
          .Should().NotBeNull()
          .And
          .Should().BeEquivalentTo(createdEvidence);

        // retrieve ByClaim
        datastore.ByClaim(createdClaim.Id)
          .Should().ContainSingle()
          .And
          .Should().BeEquivalentTo(createdEvidence);
      }
      finally
      {
        claimDatastore.Delete(createdClaim);
      }

      // delete
      datastore.ById(createdEvidence.Id)
        .Should().BeNull();
    }
  }
}
