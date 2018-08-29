using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IEvidenceDatastore<T>
  {
    IEnumerable<IEnumerable<T>> ByClaim(string claimId);
    T Create(T evidence);
    void Update(T evidence);
  }
#pragma warning restore CS1591
}
