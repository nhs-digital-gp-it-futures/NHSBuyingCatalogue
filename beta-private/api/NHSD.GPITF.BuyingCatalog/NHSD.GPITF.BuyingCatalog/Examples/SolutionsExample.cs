using NHSD.GPITF.BuyingCatalog.Models;
using Swashbuckle.AspNetCore.Examples;
using System;

namespace NHSD.GPITF.BuyingCatalog.Examples
{
#pragma warning disable CS1591
  public sealed class SolutionsExample : IExamplesProvider
  {
    public object GetExamples()
    {
      return new Solutions
      {
        Id = "A3C6830F-2E7C-4545-A4B9-02D20C4C92E1",
        OrganisationId = "067A715D-AA0D-4907-B6B8-366611CB1FC4",
        CreatedById = "70B7D8DE-494B-43AD-A713-8E6A6472FB14",
        CreatedOn = new DateTime(2017, 12, 25, 06, 45, 0),
        ModifiedById = "70B7D8DE-494B-43AD-A713-8E6A6472FB14",
        ModifiedOn = new DateTime(2018, 12, 31, 17, 25, 0),
        Version = "1.0",
        Status = SolutionStatus.Draft,
        Name = "Really Kool Document Manager",
        Description = "Does really kool document management"
      };
    }
  }
#pragma warning restore CS1591
}
