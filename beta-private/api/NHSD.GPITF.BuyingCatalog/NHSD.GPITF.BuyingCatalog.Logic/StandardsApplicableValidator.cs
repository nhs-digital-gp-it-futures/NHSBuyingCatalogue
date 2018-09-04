using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class StandardsApplicableValidator : ClaimsValidatorBase<StandardsApplicable>, IStandardsApplicableValidator
  {
    public StandardsApplicableValidator(
      IHttpContextAccessor context,
      IStandardsApplicableDatastore claimDatastore,
      ISolutionsDatastore solutionsDatastore) :
      base(context, claimDatastore, solutionsDatastore)
    {
      RuleSet(nameof(IStandardsApplicableLogic.Delete), () =>
      {
        MustBePending();
      });
    }

    private void MustBePending()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          return _context.HasRole(Roles.Supplier) &&
            (x.Status == StandardsApplicableStatus.NotStarted ||
            x.Status == StandardsApplicableStatus.Draft);
        })
        .WithMessage("Only supplier can delete a draft claim");
    }
  }
}
