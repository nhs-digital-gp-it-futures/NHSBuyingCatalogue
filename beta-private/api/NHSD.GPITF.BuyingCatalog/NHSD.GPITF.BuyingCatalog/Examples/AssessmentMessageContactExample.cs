using NHSD.GPITF.BuyingCatalog.Models;
using Swashbuckle.AspNetCore.Examples;

namespace NHSD.GPITF.BuyingCatalog.Examples
{
#pragma warning disable CS1591
  public sealed class AssessmentMessageContactExample : IExamplesProvider
  {
    public object GetExamples()
    {
      return new AssessmentMessageContact
      {
        AssessmentMessageId = "7a5b09bd-e9dd-4ee6-8505-5f5847bc5631",
        ContactId = "BFC60EC8-19EB-4FE7-8016-87A56970A0C4"
      };
    }
  }
#pragma warning restore CS1591
}
