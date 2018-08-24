using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Interfaces
{
#pragma warning disable CS1591
  public interface IClaimsLogic<T>
  {
    IQueryable<T> BySolution(string solutionId);
    T Create(T claim);
    void Update(T claim);
    void Delete(T claim);
  }
#pragma warning restore CS1591
}
