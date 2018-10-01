using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.EvidenceBlobStore.SharePoint
{
  public class EvidenceBlobStoreValidator : AbstractValidator<string>, IEvidenceBlobStoreValidator
  {
    protected readonly IHttpContextAccessor _context;

    public EvidenceBlobStoreValidator(IHttpContextAccessor context)
    {
      _context = context;

      // solutionId
      RuleSet(nameof(IEvidenceBlobStoreLogic.PrepareForSolution), () =>
      {
        MustBeAdminOrSupplier();
      });

      // claimId
      RuleSet(nameof(IEvidenceBlobStoreLogic.AddEvidenceForClaim), () =>
      {
        MustBeAdminOrSupplier();
      });

      // claimId
      RuleSet(nameof(IEvidenceBlobStoreLogic.EnumerateFolder), () =>
      {
        MustBeAdminOrSupplier();
      });
    }

    public void MustBeAdmin()
    {
      RuleFor(x => x)
        .Must(x => _context.HasRole(Roles.Admin))
        .WithMessage("Must be admin");
    }

    public void MustBeAdminOrSupplier()
    {
      RuleFor(x => x)
        .Must(x =>
          _context.HasRole(Roles.Admin) ||
          _context.HasRole(Roles.Supplier))
        .WithMessage("Must be admin or supplier");
    }
  }
}
