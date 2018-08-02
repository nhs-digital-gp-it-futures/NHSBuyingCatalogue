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
      string role = Roles.Admin)
    {
      var orgClaim = new Claim(nameof(Organisation), orgId);
      var roleClaim = new Claim(ClaimTypes.Role, role);
      var claimsIdentity = new ClaimsIdentity(new[] { orgClaim, roleClaim });
      var user = new ClaimsPrincipal(new[] { claimsIdentity });
      var ctx = new DefaultHttpContext { User = user };

      return ctx;
    }

    public static Organisation GetOrganisation(
      string id = "NHS Digital",
      string primaryRoleId = PrimaryRole.GovernmentDepartment)
    {
      return new Organisation
      {
        Id = id,
        Name = id,
        PrimaryRoleId = primaryRoleId
      };
    }

    public static Solution GetSolution(
      string id = null,
      string orgId = null,
      SolutionStatus status = SolutionStatus.Draft)
    {
      return new Solution
      {
        Id = id ?? Guid.NewGuid().ToString(),
        OrganisationId = orgId ?? Guid.NewGuid().ToString(),
        Status = status
      };
    }

    public static TechnicalContact GetTechnicalContact(
      string id = null,
      string solutionId = null
      )
    {
      return new TechnicalContact
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solutionId ?? Guid.NewGuid().ToString()
      };
    }

    public static AssessmentMessage GetAssessmentMessage(
      string id = null,
      string solutionId = null,
      string contactId = null
      )
    {
      return new AssessmentMessage
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solutionId ?? Guid.NewGuid().ToString(),
        ContactId = contactId ?? Guid.NewGuid().ToString()
      };
    }
  }
}
