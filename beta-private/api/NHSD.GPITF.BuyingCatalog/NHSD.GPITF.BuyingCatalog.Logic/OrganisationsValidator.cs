using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class OrganisationsValidator : ValidatorBase<Organisations>, IOrganisationsValidator
  {
    public OrganisationsValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IOrganisationsLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IOrganisationsLogic.Update), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IOrganisationsLogic.Delete), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
