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
  public sealed class CapabilitiesImplementedEvidenceDatastore_Tests : DatastoreBase_Tests<CapabilitiesImplementedEvidenceDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new CapabilitiesImplementedEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void CRUD_Succeeds()
    {
      var soln = Retriever.GetAllSolutions(_policy).First();
      var contact = Retriever.GetAllContacts(_policy).First(cont => cont.OrganisationId == soln.OrganisationId);
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var claimDatastore = new CapabilitiesImplementedDatastore(DatastoreBaseSetup.CrmConnectionFactory, new Mock<ILogger<CapabilitiesImplementedDatastore>>().Object, _policy);
      var datastore = new CapabilitiesImplementedEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var newClaim = new CapabilitiesImplemented
      {
        Id = Guid.NewGuid().ToString(),
        SolutionId = soln.Id,
        CapabilityId = cap.Id,
        Status = CapabilitiesImplementedStatus.Draft
      };
      Verifier.Verify(newClaim);
      var createdClaim = claimDatastore.Create(newClaim);

      // create
      var newEvidence = new CapabilitiesImplementedEvidence
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
