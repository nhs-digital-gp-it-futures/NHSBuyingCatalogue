using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableFilter : IStandardsApplicableFilter
  {
    public IEnumerable<StandardsApplicable> Filter(IEnumerable<StandardsApplicable> input)
    {
      return input;
    }
  }
}
