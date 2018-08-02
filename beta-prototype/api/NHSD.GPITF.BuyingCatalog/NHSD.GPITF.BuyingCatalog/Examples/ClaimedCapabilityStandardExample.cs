using NHSD.GPITF.BuyingCatalog.Models;
using Swashbuckle.AspNetCore.Examples;

namespace NHSD.GPITF.BuyingCatalog.Examples
{
#pragma warning disable CS1591
  public sealed class ClaimedCapabilityStandardExample : IExamplesProvider
  {
    public object GetExamples()
    {
      return new ClaimedCapabilityStandard
      {
        ClaimedCapabilityId = "0F2614F9-2521-414A-A448-0096C0DF3ABE",
        StandardId = "INT11",
        Evidence = "ClaimedCapabilityStandard Evidence"
      };
    }
  }
#pragma warning restore CS1591
}
