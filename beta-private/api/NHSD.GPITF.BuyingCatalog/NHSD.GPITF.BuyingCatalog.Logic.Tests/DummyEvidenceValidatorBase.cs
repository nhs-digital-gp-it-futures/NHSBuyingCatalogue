using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic.Tests
{
  public sealed class DummyEvidenceValidatorBase : EvidenceValidatorBase<EvidenceBase>
  {
    public DummyEvidenceValidatorBase(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
