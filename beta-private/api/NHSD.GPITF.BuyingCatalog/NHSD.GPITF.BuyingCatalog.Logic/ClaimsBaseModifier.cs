using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ClaimsBaseModifier<T> : IClaimsBaseModifier<T> where T : ClaimsBase
  {
    public void ForCreate(T input)
    {
      input.OriginalDate = DateTime.UtcNow;
    }
  }
}
