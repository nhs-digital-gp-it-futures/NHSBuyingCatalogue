using FluentValidation;
using Microsoft.AspNetCore.Http;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public sealed class ReviewsValidator : ValidatorBase<Reviews>, IReviewsValidator
  {
    public ReviewsValidator(IHttpContextAccessor context) :
      base(context)
    {
      RuleSet(nameof(IReviewsLogic.BySolution), () =>
      {
        MustBeAdminOrSupplier();
      });
      RuleSet(nameof(IReviewsLogic.Create), () =>
      {
        MustBeAdminOrSupplier();
      });
      RuleSet(nameof(IReviewsLogic.Update), () =>
      {
        MustBeAdmin();
      });
      RuleSet(nameof(IReviewsLogic.Delete), () =>
      {
        MustBeAdmin();
      });
    }

  }
}
