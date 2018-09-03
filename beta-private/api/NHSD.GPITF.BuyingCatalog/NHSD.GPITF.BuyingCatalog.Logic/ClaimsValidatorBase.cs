using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ClaimsValidatorBase<T> : ValidatorBase<T>, IClaimsValidator<T> where T : ClaimsBase
  {
    private readonly IClaimsDatastore<T> _claimDatastore;

    public ClaimsValidatorBase(
      IHttpContextAccessor context,
      IClaimsDatastore<T> claimDatastore) :
      base(context)
    {
      _claimDatastore = claimDatastore;

      RuleSet(nameof(IClaimsLogic<T>.Update), () =>
      {
        RuleForUpdate();
      });

      RuleSet(nameof(IClaimsLogic<T>.Delete), () =>
      {
        RuleForDelete();
      });

      RuleFor(x => x.Id)
        .NotNull()
        .Must(id => Guid.TryParse(id, out _))
        .WithMessage("Invalid Id");
      RuleFor(x => x.SolutionId)
        .NotNull()
        .Must(solnId => Guid.TryParse(solnId, out _))
        .WithMessage("Invalid SolutionId");
    }

    protected abstract void RuleForDelete();

    private void RuleForUpdate()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          var claim = _claimDatastore.ById(x.Id);
          return claim != null && x.SolutionId == claim.SolutionId;
        })
        .WithMessage("Cannot transfer claim between solutions");
    }
  }
}
