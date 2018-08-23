using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IReviewsDatastore<T>
  {
    IQueryable<T> ByEvidence(string evidenceId);
    T Create(T review);
  }
#pragma warning restore CS1591
}
