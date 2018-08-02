using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Extensions.Logging;
using NHSD.GPITF.BuyingCatalog.Datastore.Database.Interfaces;
using NHSD.GPITF.BuyingCatalog.Interfaces;
using NHSD.GPITF.BuyingCatalog.Models;
using System.Linq;

namespace NHSD.GPITF.BuyingCatalog.Datastore.Database
{
  public sealed class ClaimedCapabilityStandardDatastore : DatastoreBase<ClaimedCapabilityStandard>, IClaimedCapabilityStandardDatastore
  {
    public ClaimedCapabilityStandardDatastore(IDbConnectionFactory dbConnectionFactory, ILogger<ClaimedCapabilityStandardDatastore> logger, ISyncPolicyFactory policy) :
      base(dbConnectionFactory, logger, policy)
    {
    }

    public IQueryable<ClaimedCapabilityStandard> ByClaimedCapability(string claimedCapabilityId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select * from ClaimedCapabilityStandard
where ClaimedCapabilityId = '{claimedCapabilityId}'
";
        var retval = _dbConnection.Value.Query<ClaimedCapabilityStandard>(sql);
        return retval.AsQueryable();
      });
    }

    public IQueryable<ClaimedCapabilityStandard> ByStandard(string standardId)
    {
      return GetInternal(() =>
      {
        var sql = $@"
select * from ClaimedCapabilityStandard
where StandardId = '{standardId}'
";
        var retval = _dbConnection.Value.Query<ClaimedCapabilityStandard>(sql);
        return retval.AsQueryable();
      });
    }

    public ClaimedCapabilityStandard Create(ClaimedCapabilityStandard claimedCapStd)
    {
      return GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Insert(claimedCapStd, trans);
          trans.Commit();

          return claimedCapStd;
        }
      });
    }

    public void Delete(ClaimedCapabilityStandard claimedCapStd)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Delete(claimedCapStd, trans);
          trans.Commit();
          return 0;
        }
      });
    }

    public void Update(ClaimedCapabilityStandard claimedCapStd)
    {
      GetInternal(() =>
      {
        using (var trans = _dbConnection.Value.BeginTransaction())
        {
          _dbConnection.Value.Update(claimedCapStd, trans);
          trans.Commit();
          return 0;
        }
      });
    }
  }
}
