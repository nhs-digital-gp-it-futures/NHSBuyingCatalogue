using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NHSD.GPITF.BuyingCatalog.Datastore.CRM.SystemTests
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
      string primaryRoleId = PrimaryRole.GovernmentDepartment,
      string odsCode = "NHS Digital ODS Code")
    {
      var retval = new Organisations
      {
        Id = id,
        Name = id,
        PrimaryRoleId = primaryRoleId,
        OdsCode = odsCode
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static Solutions GetSolution(
      string id = null,
      string previousId = null,
      string orgId = null,
      SolutionStatus status = SolutionStatus.Draft,
      string createdById = null,
      DateTime? createdOn = null,
      string modifiedById = null,
      DateTime? modifiedOn = null)
    {
      var retval = new Solutions
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = previousId,
        OrganisationId = orgId ?? Guid.NewGuid().ToString(),
        Status = status,
        CreatedById = createdById ?? Guid.NewGuid().ToString(),
        CreatedOn = createdOn ?? DateTime.Now,
        ModifiedById = modifiedById ?? Guid.NewGuid().ToString(),
        ModifiedOn = modifiedOn ?? DateTime.Now
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static TechnicalContacts GetTechnicalContact(
      string id = null,
      string solutionId = null,
      string contactType = "Technical Contact",
      string emailAddress = "jon.dough@tpp.com"
      )
    {
      var retval = new TechnicalContacts
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solutionId ?? Guid.NewGuid().ToString(),
        ContactType = contactType,
        EmailAddress = emailAddress
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static Contacts GetContact(
      string id = null,
      string orgId = null,
      string emailAddress1 = null)
    {
      var retval = new Contacts
      {
        Id = id ?? Guid.NewGuid().ToString(),
        OrganisationId = orgId ?? Guid.NewGuid().ToString(),
        EmailAddress1 = emailAddress1 ?? "jon.dough@tpp.com"
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static CapabilitiesImplemented GetCapabilitiesImplemented(
      string id = null,
      string solnId = null,
      string claimId = null,
      CapabilitiesImplementedStatus status = CapabilitiesImplementedStatus.Draft)
    {
      var retval = new CapabilitiesImplemented
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString(),
        CapabilityId = claimId ?? Guid.NewGuid().ToString(),
        Status = status
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static StandardsApplicable GetStandardsApplicable(
      string id = null,
      string solnId = null,
      string claimId = null,
      StandardsApplicableStatus status = StandardsApplicableStatus.Draft)
    {
      var retval = new StandardsApplicable
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solnId ?? Guid.NewGuid().ToString(),
        StandardId = claimId ?? Guid.NewGuid().ToString(),
        Status = status
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static CapabilitiesImplementedEvidence GetCapabilitiesImplementedEvidence(
      string id = null,
      string prevId = null,
      string claimId = null)
    {
      var retval = new CapabilitiesImplementedEvidence
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        ClaimId = claimId ?? Guid.NewGuid().ToString()
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static StandardsApplicableEvidence GetStandardsApplicableEvidence(
      string id = null,
      string prevId = null,
      string claimId = null)
    {
      var retval = new StandardsApplicableEvidence
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        ClaimId = claimId ?? Guid.NewGuid().ToString()
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static CapabilitiesImplementedReviews GetCapabilitiesImplementedReviews(
      string id = null,
      string prevId = null,
      string evidenceId = null)
    {
      var retval = new CapabilitiesImplementedReviews
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        EvidenceId = evidenceId ?? Guid.NewGuid().ToString()
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static StandardsApplicableReviews GetStandardsApplicableReviews(
      string id = null,
      string prevId = null,
      string evidenceId = null)
    {
      var retval = new StandardsApplicableReviews
      {
        Id = id ?? Guid.NewGuid().ToString(),
        PreviousId = prevId,
        EvidenceId = evidenceId ?? Guid.NewGuid().ToString()
      };
      Verifier.Verify(retval);
      return retval;
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

      Verifier.Verify(solnEx);
      return solnEx;
    }
  }
}
