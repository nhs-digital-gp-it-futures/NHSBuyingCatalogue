using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
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
  }
}
