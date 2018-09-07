using FluentValidation;
using NHSD.GPITF.BuyingCatalog.Models;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public interface IEvidenceValidator<T> : IValidator<T> where T : EvidenceBase
  {
  }
}