using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public interface IFilter<T>
  {
    IQueryable<T> Filter(IQueryable<T> input);
  }
}
