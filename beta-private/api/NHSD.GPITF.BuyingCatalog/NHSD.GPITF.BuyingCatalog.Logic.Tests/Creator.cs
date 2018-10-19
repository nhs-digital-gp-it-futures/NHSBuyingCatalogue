using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  internal static class Creator
  {
    public static DefaultHttpContext GetContext(
      string orgId = "NHS Digital",
      string role = Roles.Admin,
      string email = "NHS-GPIT@WigglyAmps.com")
    {
      var orgClaim = new Claim(nameof(Organisations), orgId);
      var roleClaim = new Claim(ClaimTypes.Role, role);
      var emailClaim = new Claim(ClaimTypes.Email, email);
      var claimsIdentity = new ClaimsIdentity(new[] { orgClaim, roleClaim, emailClaim });
      var user = new ClaimsPrincipal(new[] { claimsIdentity });
      var ctx = new DefaultHttpContext { User = user };

      return ctx;
    }

    public static Organisations GetOrganisation(
      string id = "NHS Digital",
      string primaryRoleId = PrimaryRole.GovernmentDepartment)
    {
      return new Organisations
      {
        Id = id,
        Name = id,
        PrimaryRoleId = primaryRoleId
      };
    }

    public static Solutions GetSolution(
      string id = null,
      string previousId = null,
      string orgId = null,
      SolutionStatus status = SolutionStatus.Draft)
    {
      return new Solutions
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = previousId,
        OrganisationId = orgId ?? Guid.NewGuid().ToString(),
        Status = status
      };
    }

    public static TechnicalContacts GetTechnicalContact(
      string id = null,
      string solutionId = null
      )
    {
      return new TechnicalContacts
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solutionId ?? Guid.NewGuid().ToString()
      };
    }

    public static ClaimsBase GetClaimsBase(
      string id = null,
      string solnId = null)
    {
      return new DummyClaimsBase
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString()
      };
    }

    public static DummyEvidenceBase GetEvidenceBase(
      string id = null,
      string prevId = null,
      string claimId = null)
    {
      return new DummyEvidenceBase
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        ClaimId = claimId ?? Guid.NewGuid().ToString()
      };
    }

    public static DummyReviewsBase GetReviewsBase(
      string id = null,
      string prevId = null,
      string evidenceId = null)
    {
      return new DummyReviewsBase
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        EvidenceId = evidenceId ?? Guid.NewGuid().ToString()
      };
    }

    public static Contacts GetContact(
      string id = null,
      string orgId = null)
    {
      return new Contacts
      {
        Id = id ?? Guid.NewGuid().ToString(),
        OrganisationId = orgId ?? Guid.NewGuid().ToString()
      };
    }

    public static CapabilitiesImplemented GetCapabilitiesImplemented(
      string id = null,
      string solnId = null,
      string claimId = null,
      CapabilitiesImplementedStatus status = CapabilitiesImplementedStatus.Draft)
    {
      return new CapabilitiesImplemented
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString(),
        CapabilityId = claimId ?? Guid.NewGuid().ToString(),
        Status = status
      };
    }

    public static StandardsApplicable GetStandardsApplicable(
      string id = null,
      string solnId = null,
      string claimId = null,
      StandardsApplicableStatus status = StandardsApplicableStatus.Draft)
    {
      return new StandardsApplicable
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString(),
        StandardId = claimId ?? Guid.NewGuid().ToString(),
        Status = status
      };
    }

    public static CapabilitiesImplementedEvidence GetCapabilitiesImplementedEvidence(
      string id = null,
      string prevId = null,
      string claimId = null)
    {
      return new CapabilitiesImplementedEvidence
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        ClaimId = claimId ?? Guid.NewGuid().ToString()
      };
    }

    public static StandardsApplicableEvidence GetStandardsApplicableEvidence(
      string id = null,
      string prevId = null,
      string claimId = null)
    {
      return new StandardsApplicableEvidence
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        ClaimId = claimId ?? Guid.NewGuid().ToString()
      };
    }

    public static CapabilitiesImplementedReviews GetCapabilitiesImplementedReviews(
      string id = null,
      string prevId = null,
      string evidenceId = null)
    {
      return new CapabilitiesImplementedReviews
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        EvidenceId = evidenceId ?? Guid.NewGuid().ToString()
      };
    }

    public static StandardsApplicableReviews GetStandardsApplicableReviews(
      string id = null,
      string prevId = null,
      string evidenceId = null)
    {
      return new StandardsApplicableReviews
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        EvidenceId = evidenceId ?? Guid.NewGuid().ToString()
      };
    }

    public static SolutionEx GetSolutionEx(
      Solutions soln = null,

      List<CapabilitiesImplemented> claimedCap = null,
      List<CapabilitiesImplementedEvidence> claimedCapEv = null,
      List<CapabilitiesImplementedReviews> claimedCapRev = null,

      List<StandardsApplicable> claimedStd = null,
      List<StandardsApplicableEvidence> claimedStdEv = null,
      List<StandardsApplicableReviews> claimedStdRev = null,

      List<TechnicalContacts> techCont = null
      )
    {
      soln = soln ?? GetSolution();

      claimedCap = claimedCap ?? new List<CapabilitiesImplemented>
      {
        GetCapabilitiesImplemented(solnId: soln.Id)
      };
      claimedCapEv = claimedCapEv ?? new List<CapabilitiesImplementedEvidence>
      {
        GetCapabilitiesImplementedEvidence(claimId: claimedCap.First().Id)
      };
      claimedCapRev = claimedCapRev ?? new List<CapabilitiesImplementedReviews>
      {
        GetCapabilitiesImplementedReviews(evidenceId: claimedCapEv.First().Id)
      };

      claimedStd = claimedStd ?? new List<StandardsApplicable>
      {
        GetStandardsApplicable(solnId: soln.Id)
      };
      claimedStdEv = claimedStdEv ?? new List<StandardsApplicableEvidence>
      {
        GetStandardsApplicableEvidence(claimId: claimedStd.First().Id)
      };
      claimedStdRev = claimedStdRev ?? new List<StandardsApplicableReviews>
      {
        GetStandardsApplicableReviews(evidenceId: claimedStdEv.First().Id)
      };

      techCont = techCont ?? new List<TechnicalContacts>
      {
        GetTechnicalContact(solutionId: soln.Id)
      };

      var solnEx = new SolutionEx
      {
        Solution = soln,

        ClaimedCapability = claimedCap,
        ClaimedCapabilityEvidence = claimedCapEv,
        ClaimedCapabilityReview = claimedCapRev,

        ClaimedStandard = claimedStd,
        ClaimedStandardEvidence = claimedStdEv,
        ClaimedStandardReview = claimedStdRev,

        TechnicalContact = techCont
      };

      return solnEx;
    }
  }
}
