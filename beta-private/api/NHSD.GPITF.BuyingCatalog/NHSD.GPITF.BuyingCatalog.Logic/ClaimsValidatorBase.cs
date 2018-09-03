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
    private readonly ISolutionsDatastore _solutionsDatastore;

    public ClaimsValidatorBase(
      IHttpContextAccessor context,
      IClaimsDatastore<T> claimDatastore,
      ISolutionsDatastore solutionsDatastore) :
      base(context)
    {
      _claimDatastore = claimDatastore;
      _solutionsDatastore = solutionsDatastore;

      RuleSet(nameof(IClaimsLogic<T>.Update), () =>
      {
        MustBeSameSolution();
        MustBeSameOrganisation();
      });

      RuleSet(nameof(IClaimsLogic<T>.Delete), () =>
      {
        MustBeSameOrganisation();
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

    private void MustBeSameOrganisation()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          var orgId = _context.OrganisationId();
          var claim = _claimDatastore.ById(x.Id);
          if (claim == null)
          {
            return false;
          }
          var claimSoln = _solutionsDatastore.ById(claim.SolutionId);
          return claimSoln != null && claimSoln.OrganisationId == orgId;
        })
        .WithMessage("Cannot update/delete claim for other organisation");
    }

    private void MustBeSameSolution()
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
