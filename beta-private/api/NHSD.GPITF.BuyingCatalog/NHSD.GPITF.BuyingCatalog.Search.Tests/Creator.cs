using NHSD.GPITF.BuyingCatalog.Logic;
using NHSD.GPITF.BuyingCatalog.Models;
using NHSD.GPITF.BuyingCatalog.Models.Porcelain;
using System;

namespace NHSD.GPITF.BuyingCatalog.Search.Tests
{
  internal static class Creator
  {
    public static Frameworks GetFramework(
      string id = null)
    {
      var retval = new Frameworks
      {
        Id = id ?? Guid.NewGuid().ToString()
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

    public static SolutionEx GetSolutionEx(
      Solutions solution = null)
    {
      var retval = new SolutionEx
      {
        Solution = solution ?? Creator.GetSolution()
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static Capabilities GetCapability(
      string id = null,
      string name = null,
      string description = null)
    {
      var retval = new Capabilities
      {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = name ?? string.Empty,
        Description = description ?? string.Empty
      };
      Verifier.Verify(retval);
      return retval;
    }

    public static CapabilitiesImplemented GetClaimedCapability(
      string id = null,
      string solutionId = null,
      string capabilityId = null)
    {
      var retval = new CapabilitiesImplemented
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solutionId ?? Guid.NewGuid().ToString(),
        CapabilityId = capabilityId ?? Guid.NewGuid().ToString()
      };
      Verifier.Verify(retval);
      return retval;
    }
  }
}
