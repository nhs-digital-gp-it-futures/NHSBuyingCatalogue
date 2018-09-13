using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class UserInfoResponseDatastore : DatastoreBase<CachedUserInfoResponseJson>, IUserInfoResponseDatastore
  {
    public UserInfoResponseDatastore(
      IDbConnectionFactory dbConnectionFactory,
      ILogger<DatastoreBase<CachedUserInfoResponseJson>> logger,
      ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public void Add(string bearerToken, string jsonCachedResponse)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          var entity = new CachedUserInfoResponseJson { Id = bearerToken, Data = jsonCachedResponse };
          _dbConnection.Value.Insert(entity, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void Remove(string bearerToken)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          var entity = new CachedUserInfoResponseJson { Id = bearerToken };
          _dbConnection.Value.Delete(entity, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public bool TryGetValue(string bearerToken, out string jsonCachedResponse)
    {
      var entity = GetInternal(() => _dbConnection.Value.Get<CachedUserInfoResponseJson>(bearerToken));
      jsonCachedResponse = entity?.Data;
      return entity != null;
    }
  }
}
