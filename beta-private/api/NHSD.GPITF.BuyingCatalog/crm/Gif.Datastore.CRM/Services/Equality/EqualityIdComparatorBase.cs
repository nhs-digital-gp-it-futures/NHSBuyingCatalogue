using System;
using System.Collections.Generic;
using Gif.Service.Contracts;

namespace Gif.Service.Services.Equality
{
    public abstract class EqualityIdComparatorBase<T> : IEqualityComparer<T> where T : IHasId
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
