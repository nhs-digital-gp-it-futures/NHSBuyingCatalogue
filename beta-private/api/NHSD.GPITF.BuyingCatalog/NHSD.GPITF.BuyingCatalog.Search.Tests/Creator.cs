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
      return new Frameworks
      {
        Id = id ?? Guid.NewGuid().ToString()
      };
    }

    public static Solutions GetSolution(
      string id = null,
      string name = null,
      string description = null)
    {
      return new Solutions
      {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = name ?? string.Empty,
        Description = description ?? string.Empty
      };
    }

    public static SolutionEx GetSolutionEx(
      Solutions solution = null)
    {
      return new SolutionEx
      {
        Solution = solution ?? Creator.GetSolution()
      };
    }

    public static Capabilities GetCapability(
      string id = null,
      string name = null,
      string description = null)
    {
      return new Capabilities
      {
        Id = id ?? Guid.NewGuid().ToString(),
        Name = name ?? string.Empty,
        Description = description ?? string.Empty
      };
    }

    public static CapabilitiesImplemented GetClaimedCapability(
      string id = null,
      string solutionId = null,
      string capabilityId = null)
    {
      return new CapabilitiesImplemented
      {
        Id = id ?? Guid.NewGuid().ToString(),
        SolutionId = solutionId ?? Guid.NewGuid().ToString(),
        CapabilityId = capabilityId ?? Guid.NewGuid().ToString()
      };
    }
  }
}
