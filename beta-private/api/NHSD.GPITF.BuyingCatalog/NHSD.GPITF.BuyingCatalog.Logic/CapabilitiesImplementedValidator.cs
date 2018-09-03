using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class CapabilitiesImplementedValidator : ClaimsValidatorBase<CapabilitiesImplemented>, ICapabilitiesImplementedValidator
  {
    public CapabilitiesImplementedValidator(
      IHttpContextAccessor context,
      ICapabilitiesImplementedDatastore claimDatastore,
      ISolutionsDatastore solutionsDatastore) :
      base(context, claimDatastore, solutionsDatastore)
    {
      RuleSet(nameof(ICapabilitiesImplementedLogic.Delete), () =>
      {
        RuleForDelete();
      });
    }

    private void RuleForDelete()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          return _context.HasRole(Roles.Supplier) &&
            x.Status == CapabilitiesImplementedStatus.Draft;
        })
        .WithMessage("Only supplier can delete a draft claim");
    }
  }
}
