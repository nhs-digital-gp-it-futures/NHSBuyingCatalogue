using Gif.Service.Contracts;
using System.Collections.Generic;

namespace Gif.Service.Services.Equality
{
  public class EqualityIdComparatorBase<T> : IEqualityComparer<T> where T : IHasId
    {

        public bool Equals(T x, T y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(T obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
