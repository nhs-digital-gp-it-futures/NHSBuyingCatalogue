using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableModifier : ClaimsBaseModifier<StandardsApplicable>, IStandardsApplicableModifier
  {
    public void ForUpdate(StandardsApplicable input)
    {
      if (input.Status == StandardsApplicableStatus.Submitted)
      {
        input.SubmittedOn = DateTime.UtcNow;
      }
    }
  }
}
