using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableEvidenceFilter : EvidenceFilterBase<IEnumerable<StandardsApplicableEvidence>>, IStandardsApplicableEvidenceFilter
  {
    public StandardsApplicableEvidenceFilter(IHttpContextAccessor context) :
      base(context)
    {
    }

    protected override IEnumerable<StandardsApplicableEvidence> Filter(IEnumerable<StandardsApplicableEvidence> input)
    {
      return input;
    }
  }
}
