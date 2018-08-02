using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class OrganisationValidator : ValidatorBase<Organisation>, IOrganisationValidator
  {
    public OrganisationValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IOrganisationLogic.Create), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IOrganisationLogic.Update), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IOrganisationLogic.Delete), () =>
      {
        MustBeAdmin();
      });
    }
  }
}
