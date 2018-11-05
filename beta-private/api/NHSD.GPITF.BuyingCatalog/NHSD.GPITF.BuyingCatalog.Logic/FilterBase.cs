using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace NHSD.GPITF.BuyingCatalog.Logic
{
  public abstract class FilterBase<T> : IFilter<T>
  {
    protected readonly IHttpContextAccessor _context;

    public FilterBase(IHttpContextAccessor context)
    {
      _context = context;
    }

    public abstract T Filter(T input);

    private T FilterInternal(T input)
    {
      Verify(input);

      return Filter(input);
    }

    public IEnumerable<T> Filter(IEnumerable<T> input)
    {
      return input.Select(x => FilterInternal(x)).Where(x => x != null);
    }

    private static void Verify(object obj)
    {
      if (obj == null)
      {
        return;
      }

      var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (var prop in props)
      {
        if (prop.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)) &&
          prop.PropertyType != typeof(string))
        {
          foreach (var item in (IEnumerable)prop.GetValue(obj, null))
          {
            Verify(item);
          }
        }

        var required = prop.GetCustomAttribute<RequiredAttribute>();
        if (required != null && !required.AllowEmptyStrings)
        {
          var value = prop.GetValue(obj) as string;
          if (value != null &&
            string.IsNullOrWhiteSpace(value))
          {
            throw new InvalidOperationException($"Value is null or whitespace:  {prop.ReflectedType}.{prop.Name}");
          }
        }
      }
    }
  }
}
