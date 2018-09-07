using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedEvidenceFilter : EvidenceFilterBase<IEnumerable<CapabilitiesImplementedEvidence>>, ICapabilitiesImplementedEvidenceFilter
  {
    public CapabilitiesImplementedEvidenceFilter(IHttpContextAccessor context) :
      base(context)
    {
    }

    protected override IEnumerable<CapabilitiesImplementedEvidence> Filter(IEnumerable<CapabilitiesImplementedEvidence> input)
    {
      return input;
    }
  }
}
