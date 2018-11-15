using System.Collections.Generic;

namespace Gif.Service.Contracts
{
#pragma warning disable CS1591
  public interface IReviewsDatastore<T>
  {
    IEnumerable<IEnumerable<T>> ByEvidence(string evidenceId);
    T ById(string id);
    T Create(T review);
    void Delete(T review);
  }
#pragma warning restore CS1591
}
