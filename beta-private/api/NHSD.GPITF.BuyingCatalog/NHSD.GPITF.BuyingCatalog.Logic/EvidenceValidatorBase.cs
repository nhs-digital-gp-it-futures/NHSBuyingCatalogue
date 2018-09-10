using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class EvidenceValidatorBase<T> : ValidatorBase<T>, IEvidenceValidator<T> where T : EvidenceBase
  {
    public EvidenceValidatorBase(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IEvidenceLogic<T>.Create), () =>
      {
        MustBeValidClaimId();
        MustBeSupplier();
      });
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
  }
}
