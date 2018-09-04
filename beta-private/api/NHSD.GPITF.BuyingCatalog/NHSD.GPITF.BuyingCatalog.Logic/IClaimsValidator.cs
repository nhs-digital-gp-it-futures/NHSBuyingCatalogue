using FluentValidation;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public interface IClaimsValidator<T> : IValidator<T> where T : ClaimsBase
  {
  }
}
