using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedFilter : ICapabilitiesImplementedFilter
  {
    public IEnumerable<CapabilitiesImplemented> Filter(IEnumerable<CapabilitiesImplemented> input)
    {
      return input;
    }
  }
}
