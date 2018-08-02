using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class TechnicalContactValidator : ValidatorBase<TechnicalContact>, ITechnicalContactValidator
  {
    private readonly ISolutionDatastore _solutionDatastore;

    public TechnicalContactValidator(
      IHttpContextAccessor context,
      ISolutionDatastore solutionDatastore) :
      base(context)
    {
      _solutionDatastore = solutionDatastore;

      RuleSet(nameof(ITechnicalContactLogic.Create), () =>
      {
        MustBeAdminOrSupplier();
        SupplierOwn();
      });
      RuleSet(nameof(ITechnicalContactLogic.Update), () =>
      {
        MustBeAdminOrSupplier();
        SupplierOwn();
      });
      RuleSet(nameof(ITechnicalContactLogic.Delete), () =>
      {
        MustBeAdminOrSupplier();
        SupplierOwn();
      });
    }

    private void SupplierOwn()
    {
      RuleFor(x => x)
        .Must(x => 
        {
          if (_context.HasRole(Roles.Supplier))
          {
            var soln = _solutionDatastore.ById(x.SolutionId);
            return _context.ContextOrganisationId() == soln.OrganisationId;
          }
          return _context.HasRole(Roles.Admin);
        })
        .WithMessage("Supplier can only change own Technical Contacts");
    }
  }
}
