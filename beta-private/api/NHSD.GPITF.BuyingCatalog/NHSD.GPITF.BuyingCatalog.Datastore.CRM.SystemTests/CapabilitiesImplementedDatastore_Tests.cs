using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
{
  [TestFixture]
  public sealed class CapabilitiesImplementedDatastore_Tests : DatastoreBase_Tests<CapabilitiesImplementedDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new CapabilitiesImplementedDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void BySolution_ReturnsData()
    {
      var allSolns = Retriever.GetAllSolutions(_policy);
      var ids = allSolns.Select(soln => soln.Id).Distinct();
      var datastore = new CapabilitiesImplementedDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      var datas = ids.ToList().SelectMany(id => datastore.BySolution(id));

      datas.Should().NotBeEmpty();
      datas.ToList().ForEach(data => Verifier.Verify(data));
    }

    [Test]
    public void CRUD_Succeeds()
    {
      var soln = Retriever.GetAllSolutions(_policy).First();
      var cap = Retriever.GetAllCapabilities(_policy).First();
      var datastore = new CapabilitiesImplementedDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy);

      // create
      var newEnt = new CapabilitiesImplemented
      {
        Id = Guid.NewGuid().ToString(),
        SolutionId = soln.Id,
        CapabilityId = cap.Id,
        Status = CapabilitiesImplementedStatus.Draft
      };
      Verifier.Verify(newEnt);
      var createdEnt = datastore.Create(newEnt);
      createdEnt.Should().BeEquivalentTo(newEnt, opt => opt.Excluding(ent => ent.Id));

      try
      {
        // update
        createdEnt.Status = CapabilitiesImplementedStatus.Submitted;
        datastore.Update(createdEnt);

        // retrieve
        datastore.ById(createdEnt.Id)
          .Should().BeEquivalentTo(createdEnt);
      }
      finally
      {
        // delete
        datastore.Delete(createdEnt);
      }

      // delete
      datastore.ById(createdEnt.Id)
        .Should().BeNull();
    }
  }

  [TestFixture]
  public sealed class CapabilitiesImplementedEvidenceDatastore_Tests : DatastoreBase_Tests<CapabilitiesImplementedEvidenceDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new CapabilitiesImplementedEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void Crud_Succeeds()
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
      createdEvidence.Should().BeEquivalentTo(newEvidence, opt => opt.Excluding(ev => ev.Id));

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

  [TestFixture]
  public sealed class StandardsApplicableEvidenceDatastore_Tests : DatastoreBase_Tests<StandardsApplicableEvidenceDatastore>
  {
    [Test]
    public void Constructor_Completes()
    {
      Assert.DoesNotThrow(() => new StandardsApplicableEvidenceDatastore(DatastoreBaseSetup.CrmConnectionFactory, _logger, _policy));
    }

    [Test]
    public void Crud_Succeeds()
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
      createdEvidence.Should().BeEquivalentTo(newEvidence, opt => opt.Excluding(ev => ev.Id));

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
