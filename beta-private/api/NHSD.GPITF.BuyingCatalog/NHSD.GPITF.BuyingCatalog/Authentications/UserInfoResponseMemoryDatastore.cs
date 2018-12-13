using NHSD.GPITF.BuyingCatalog.Interfaces;
using System.Collections.Generic;

namespace NHSD.GPITF.BuyingCatalog.Authentications
{
#pragma warning disable CS1591
  public sealed class UserInfoResponseMemoryDatastore : IUserInfoResponseDatastore
  {
    private readonly object _cacheLock = new object();

    // [bearerToken]-->[UserInfoResponse]
    private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

    public void SafeAdd(string bearerToken, string jsonCachedResponse)
    {
      lock (_cacheLock)
      {
        _cache.Remove(bearerToken);
        _cache.Add(bearerToken, jsonCachedResponse);
      }
    }

    public void Remove(string bearerToken)
    {
      lock (_cacheLock)
      {
        _cache.Remove(bearerToken);
      }
    }

    public bool TryGetValue(string bearerToken, out string jsonCachedResponse)
    {
      lock (_cacheLock)
      {
        return _cache.TryGetValue(bearerToken, out jsonCachedResponse);
      }
    }
  }
#pragma warning restore CS1591
}
