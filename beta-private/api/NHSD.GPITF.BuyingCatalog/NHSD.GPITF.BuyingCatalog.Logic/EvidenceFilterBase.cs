using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class EvidenceFilterBase<T> : FilterBase<T>, IFilter<T> where T : IEnumerable<EvidenceBase>
  {
    public EvidenceFilterBase(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
