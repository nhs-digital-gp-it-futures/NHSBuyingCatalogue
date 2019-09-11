using Gif.Service.Contracts;
using Gif.Service.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gif.Datastore.CRM.Services.Equality
{
  // stolen from:
  //    NHSD.GPITF.BuyingCatalog.Logic.Porcelain
  public class EqualityComparerBase<T> : IEqualityComparer<T> where T : IHasId
  {
    public bool Equals(T x, T y)
    {
      // remove all DateTime properties due to different handling of UTC in client and server
      var xJobj = RemoveDateTimeProperties(x);
      var yJobj = RemoveDateTimeProperties(y);
      var xJson = JsonConvert.SerializeObject(xJobj);
      var yJson = JsonConvert.SerializeObject(yJobj);

      return xJson == yJson;
    }

    private JObject RemoveDateTimeProperties(T obj)
    {
      var dateProps = obj
        .GetType()
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Where(prop => prop.PropertyType == typeof(DateTime))
        .ToList();
      var jobj = JObject.FromObject(obj);
      dateProps.ForEach(dt => jobj.Remove(dt.Name));

      return jobj;
    }

    public int GetHashCode(T obj)
    {
      return obj.Id.GetHashCode();
    }
  }

  public sealed class ClaimedCapabilityComparer : EqualityComparerBase<CapabilityImplemented>
  {
  }

  public sealed class ClaimedCapabilityEvidenceComparer : EqualityComparerBase<CapabilityImplementedEvidence>
  {
  }

  public sealed class ClaimedStandardComparer : EqualityComparerBase<StandardApplicable>
  {
  }

  public sealed class ClaimedStandardEvidenceComparer : EqualityComparerBase<StandardApplicableEvidence>
  {
  }
}
