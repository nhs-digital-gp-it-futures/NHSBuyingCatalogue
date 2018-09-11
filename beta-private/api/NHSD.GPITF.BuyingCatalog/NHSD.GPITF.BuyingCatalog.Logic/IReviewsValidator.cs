using FluentValidation;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public interface IReviewsValidator<T> : IValidator<T> where T : ReviewsBase
  {
  }
}