using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class EvidenceValidatorBase<T> : ValidatorBase<T>, IEvidenceValidator<T> where T : EvidenceBase
  {
    public EvidenceValidatorBase(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
