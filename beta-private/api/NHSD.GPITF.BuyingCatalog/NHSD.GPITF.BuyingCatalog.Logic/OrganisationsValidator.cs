using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class OrganisationsValidator : ValidatorBase<Organisations>, IOrganisationsValidator
  {
    public OrganisationsValidator(IHttpContextAccessor context) :
      base(context)
    {
    }
  }
}
