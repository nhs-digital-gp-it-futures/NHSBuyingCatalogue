using Newtonsoft.Json;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Logic.Porcelain
{
  public abstract class EqualityComparerBase<T> : IEqualityComparer<T> where T : IHasId
  {
    public bool Equals(T x, T y)
    {
      var xJson = JsonConvert.SerializeObject(x);
      var yJson = JsonConvert.SerializeObject(y);

      return xJson == yJson;
    }

    public int GetHashCode(T obj)
    {
      return obj.Id.GetHashCode();
    }
  }
}
