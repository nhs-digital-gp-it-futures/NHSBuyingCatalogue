using NHSD.GPITF.BuyingCatalog.Models;
using Swashbuckle.AspNetCore.Examples;
using System;

namespace NHSD.GPITF.BuyingCatalog.Examples
{
#pragma warning disable CS1591
  public sealed class AssessmentMessageExample : IExamplesProvider
  {
    public object GetExamples()
    {
      return new AssessmentMessage
      {
        Id = "4938DB1A-8AFA-4901-8195-E2A0A7EA939D",
        ContactId = "9C6A646F-09B7-46E9-969E-DA63D3C77E48",
        SolutionId = "C8D558DA-8EC9-4E36-881A-344F0F852284",
        Timestamp = DateTime.UtcNow,
        Message = "another assessment message"
      };
    }
  }
#pragma warning restore CS1591
}
