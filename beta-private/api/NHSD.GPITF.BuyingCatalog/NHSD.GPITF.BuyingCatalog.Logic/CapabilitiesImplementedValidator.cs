using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedValidator : ClaimsValidatorBase<CapabilitiesImplemented>, ICapabilitiesImplementedValidator
  {
    public CapabilitiesImplementedValidator(
      IHttpContextAccessor context,
      ICapabilitiesImplementedDatastore claimDatastore) :
      base(context, claimDatastore)
    {
    }
  }
}
