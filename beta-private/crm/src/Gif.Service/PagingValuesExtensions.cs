#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Gif.Service.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using Gif.Service.Contracts;

namespace Gif.Service
{
  public static class PagingValuesExtensions
  {
    public static IEnumerable<T> GetPagingValues<T>(this IEnumerable<T> stuff, int? pageIndex, int? pageSize, IEnumerable<T> items, out int totalPages)
    {
      var skipPage = Paging.DefaultSkip;

      if (pageIndex != null && pageIndex != 1)
        skipPage = (int)pageIndex;

      var skipValue = pageSize ?? Paging.DefaultSkip;
      pageSize = pageSize ?? Paging.DefaultPageSize;
      totalPages = (int)Math.Ceiling(Convert.ToInt32(items.Count()) / Convert.ToDecimal(pageSize));

      skipPage--;

      if (totalPages == 0 && items.Any())
        totalPages = 1;

      return items.Skip(skipPage * skipValue).Take((int)pageSize);
    }

  }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
