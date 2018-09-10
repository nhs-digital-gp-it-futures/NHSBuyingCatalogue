using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class EvidenceValidatorBase<T> : ValidatorBase<T>, IEvidenceValidator<T> where T : EvidenceBase
  {
    private readonly IClaimsDatastore<ClaimsBase> _claimDatastore;
    private readonly ISolutionsDatastore _solutionDatastore;

    public EvidenceValidatorBase(
      IClaimsDatastore<ClaimsBase> claimDatastore,
      ISolutionsDatastore solutionDatastore,
      IHttpContextAccessor context) :
      base(context)
    {
      _claimDatastore = claimDatastore;
      _solutionDatastore = solutionDatastore;

      RuleSet(nameof(IEvidenceLogic<T>.Create), () =>
      {
        MustBeValidClaimId();
        MustBeSupplier();
        SolutionMustBeInReview();
        MustBeFromSameOrganisation();
      });
    }

    protected abstract SolutionStatus SolutionReviewStatus { get; }

    internal void SolutionMustBeInReview()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          var claim = _claimDatastore.ById(x.ClaimId);
          var soln = _solutionDatastore.ById(claim.SolutionId);
          return soln.Status == SolutionReviewStatus;
        })
        .WithMessage("Can only add evidence if solution is in review");
    }

    internal void MustBeValidClaimId()
    {
      RuleFor(x => x.ClaimId)
        .NotNull()
        .Must(id => Guid.TryParse(id, out _))
        .WithMessage("Invalid ClaimId");
    }

    internal void MustBeSupplier()
    {
      RuleFor(x => x)
        .Must(x => _context.HasRole(Roles.Supplier))
        .WithMessage("Must be supplier");
    }

    internal void MustBeFromSameOrganisation()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          var claim = _claimDatastore.ById(x.ClaimId);
          var soln = _solutionDatastore.ById(claim.SolutionId);
          var orgId = _context.OrganisationId();
          return soln.OrganisationId == orgId;
        })
        .WithMessage("Must be from same organisation");
    }
  }
}
