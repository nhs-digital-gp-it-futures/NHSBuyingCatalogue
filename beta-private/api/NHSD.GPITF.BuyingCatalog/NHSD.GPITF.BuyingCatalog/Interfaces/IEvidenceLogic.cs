using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IEvidenceLogic<T>
  {
    IQueryable<T> ByClaim(string claimId);
    T Create(T evidence);
  }
#pragma warning restore CS1591
}
