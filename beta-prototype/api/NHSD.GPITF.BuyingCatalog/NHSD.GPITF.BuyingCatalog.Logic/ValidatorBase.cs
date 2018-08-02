using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class ValidatorBase<T> : AbstractValidator<T>
  {
    protected readonly IHttpContextAccessor _context;

    public ValidatorBase(IHttpContextAccessor context)
    {
      _context = context;
    }

    protected void MustBeAdmin()
    {
      RuleFor(x => x)
        .Must(x => _context.HasRole(Roles.Admin))
        .WithMessage("Must be admin");
    }

    protected void MustBeAdminOrSupplier()
    {
      RuleFor(x => x)
        .Must(x =>
          _context.HasRole(Roles.Admin) ||
          _context.HasRole(Roles.Supplier))
        .WithMessage("Must be admin or supplier");
    }
  }
}
