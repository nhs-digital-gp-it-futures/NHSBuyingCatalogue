using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class EvidenceValidatorBase<T> : ValidatorBase<T>, IEvidenceValidator<T> where T : EvidenceBase
  {
    private readonly IEvidenceDatastore<T> _evidenceDatastore;
    private readonly IClaimsDatastore<ClaimsBase> _claimDatastore;
    private readonly ISolutionsDatastore _solutionDatastore;

    public EvidenceValidatorBase(
      IEvidenceDatastore<T> evidenceDatastore,
      IClaimsDatastore<ClaimsBase> claimDatastore,
      ISolutionsDatastore solutionDatastore,
      IHttpContextAccessor context) :
      base(context)
    {
      _evidenceDatastore = evidenceDatastore;
      _claimDatastore = claimDatastore;
      _solutionDatastore = solutionDatastore;

      RuleSet(nameof(IEvidenceLogic<T>.Create), () =>
      {
        MustBeValidClaimId();
        MustBeSupplier();
        SolutionMustBeInReview();
        MustBeFromSameOrganisation();
        MustBeValidPreviousId();
        PreviousMustBeForSameClaim();
        PreviousMustNotBeInUse();
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

    internal void MustBeValidPreviousId()
    {
      RuleFor(x => x.PreviousId)
        .Must(id => Guid.TryParse(id, out _))
        .When(x => !string.IsNullOrEmpty(x.PreviousId))
        .WithMessage("Invalid PreviousId");
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

    internal void PreviousMustBeForSameClaim()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          var evidence = _evidenceDatastore.ById(x.PreviousId);
          return x.ClaimId == evidence.ClaimId;
        })
        .When(x => !string.IsNullOrEmpty(x.PreviousId))
        .WithMessage("Previous evidence must be for same claim");
    }

    internal void PreviousMustNotBeInUse()
    {
      RuleFor(x => x)
        .Must(x =>
        {
          var chains = _evidenceDatastore.ByClaim(x.ClaimId);
          var allPrevIds = chains.SelectMany(chain => chain.Select(evidence => evidence.PreviousId));
          return !allPrevIds.Contains(x.PreviousId);
        })
        .When(x => !string.IsNullOrEmpty(x.PreviousId))
        .WithMessage("Previous evidence already in use");
    }
  }
}
