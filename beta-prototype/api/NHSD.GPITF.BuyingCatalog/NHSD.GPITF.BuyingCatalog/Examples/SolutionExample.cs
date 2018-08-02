using NHSD.GPITF.BuyingCatalog.Models;
using Swashbuckle.AspNetCore.Examples;

namespace NHSD.GPITF.BuyingCatalog.Examples
{
#pragma warning disable CS1591
  public sealed class SolutionExample : IExamplesProvider
  {
    public object GetExamples()
    {
      return new Solution
      {
        Id = "A3C6830F-2E7C-4545-A4B9-02D20C4C92E1",
        OrganisationId = "067A715D-AA0D-4907-B6B8-366611CB1FC4",
        Version = "1.0",
        Status = SolutionStatus.Draft,
        Name = "Really Kool Document Manager",
        Description = "Does really kool document management"
      };
    }
  }
#pragma warning restore CS1591
}
